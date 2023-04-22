using DigiSeatShared;
using DigiSeatShared.Implementations.BackgroundWorker;
using DigiSeatShared.Implementations.Lights;
using DigiSeatShared.Interfaces;
using DigiSeatShared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.Web.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DigiSeatBack
{
    public sealed partial class MainPage : Page
    {
        private List<LayoutTable> _tables = new List<LayoutTable>();
        private bool _editing = false;
        private bool _assigning = false;
        private int? _assigningEmpId;
        private List<int> _assigningTableIds = new List<int>();
        private string _editButtonText = "Edit Layout";
        private ILightIntegration _lightIntegration;
        private int _seatTableSize = 0;
        private int _seatTableId = 0;
        private int _seatTableCapacity = 2;
        private List<Section> _sectionList = new List<Section>();
        private Restaurant _restaurant;
        private ObservableCollection<Staff> _staffList = new ObservableCollection<Staff>();
        private Dialogues dialogues;
        private Integration integration;
        private Helper mpHelper;


        public MainPage()
        {
            this.InitializeComponent();
            _lightIntegration = new PlayBulb();
            integration = new Integration();
            dialogues = new Dialogues();
            mpHelper = new Helper();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await GetRestaurant();
            DrawLayout();
            //mpHelper.addOnShiftStaff(ref _staffList, _restaurant);
            beginLightReader();
            redrawThreads();
        }

        private async void beginLightReader()
        {
            try
            {
                LightQueueReader.StartReader();
            }
            catch (Exception ex)
            {
                await dialogues.ShowErrorDialog("We are having trouble using Bluetooth right now.");
            }

        }

        private void redrawThreads()
        {
            TimeSpan period = TimeSpan.FromSeconds(30);
            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, 
                    async () =>
                    {
                        var tables = await integration.GetTables();
                        RebuildLayout(tables);
                        CheckLights(tables);
                    });
            }, period);
        }

        private async Task GetRestaurant() //IMO, ideally we will use a 2nd method inside of this, (or after a line that calles this method) for the 'staff adding' and color thing, b/c our GetRestaurant isn't an accurate name to describe of what it is doing, different type of work.
        {
            try
            {
                var rest = await integration.GetRestaurant();
                _restaurant = rest;

                if (_restaurant.Staff.Any())
                {
                    foreach (var staff in rest.Staff)
                    {
                        if (staff.State == "on")
                        {
                            var indexStaff = _staffList.FirstOrDefault(x => x.Id == staff.Id);

                            var staffInList = -1;
                            if(indexStaff != null)
                            {
                                staffInList = _staffList.IndexOf(indexStaff);
                            }

                            if(staffInList == -1)
                            {
                                _staffList.Add(staff);
                            }
                            else
                            {
                                _staffList[staffInList] = staff;
                            }
                        }

                        else
                        {
                            _staffList.Remove(staff);
                        }

                    }
                    foreach (var staff in _staffList)
                    {
                        var color = ColorHelper.GetBorderColor(_staffList.IndexOf(staff));
                        if(staff.Brush != color)
                        {
                            staff.Brush = color;
                        }
                    }
                    Bindings.Update();

                    foreach (var s  in _staffList)
                    {
                        Debug.WriteLine("Staff member: " + s.Name + " " + s.Id);
                        if (s.Tables != null && s.Tables.Any())
                        {
                            foreach (var sTable in s.Tables)
                            {
                                Debug.WriteLine("    Table id: " + sTable);
                            }
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                await dialogues.ShowErrorDialog("Unable to retrieve Restaurant.");
            }
        }

        private async Task DrawLayout()
        {
            var tables = await integration.GetTables();
            if (tables != null && tables.Any())
            {
                foreach (var table in tables)
                {
                    var layoutTable = BuildLayoutTable(table);
                    _tables.Add(layoutTable);
                }
            }
        }

        private LayoutTable BuildLayoutTable(Table table)
        {
            var dragTrans = new TranslateTransform();
            dragTrans.X = table.XCoordinate;
            dragTrans.Y = table.YCoordinate;

            var rect = mpHelper.GetRect(table);
            rect.Child = new TextBlock { Text = mpHelper.GetTableText(table) };
            rect.ManipulationDelta += touchBorder_ManipulationDelta;
            rect.RenderTransform = dragTrans;
            rect.Tapped += Table_Tapped;
            rect.RightTapped += Table_RightTapped;
            Debug.WriteLine("rect.BorderBrush: " + rect.BorderBrush );

            var layoutTable = new LayoutTable { Rectangle = rect, Transform = dragTrans, Table = table };
            baseGrid.Children.Add(rect);
            AssignTableBorderColor(layoutTable, _staffList.ToList());

            return layoutTable;
        }

        private async void Table_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var rect = (Border)sender;
            var layoutTable = _tables.FirstOrDefault(x => x.Rectangle.Name == rect.Name);

            var dialog = new ContentDialog // Move to Dialogues.cs ??????
            {
                Content = $"Edit Table {layoutTable.Table.Number}?",
                IsSecondaryButtonEnabled = false,
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Primary
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Frame.Navigate(typeof(CreateTable), layoutTable.Table, new DrillInNavigationTransitionInfo());
            }

        }

        private void touchBorder_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!_editing)
            {
                return;
            }

            var rect = (Border)sender;
            var index = _tables.FindIndex(x => x.Rectangle.Name == rect.Name);
            _tables[index].Transform.X += e.Delta.Translation.X;
            _tables[index].Transform.Y += e.Delta.Translation.Y;
        }

        private async Task RebuildLayout(List<Table> updateTables)
        {
            var uiTables = new List<Border>();
            foreach (var element in baseGrid.Children)
            {
                if (element is Border ele)
                {
                    uiTables.Add(ele);
                }
            }

            foreach (var table in updateTables)
            {
                var layoutTable = _tables.FirstOrDefault(x => x.Table.Id == table.Id);

                //This table was just created. We need to add it to the UI
                if (layoutTable == null)
                {
                    var newTable = BuildLayoutTable(table);
                    _tables.Add(newTable);
                    continue;
                }
                var uiTable = uiTables.FirstOrDefault(x => x.Name == layoutTable.Rectangle.Name);
                uiTable.Background = ColorHelper.GetTableColor(table.State);
                layoutTable.Table = table;
                var textblock = uiTable.Child as TextBlock;
                textblock.Text = mpHelper.GetTableText(table);
                AssignTableBorderColor(layoutTable, _staffList.ToList());

            }
        }

        private async void Save_Tapped(object sender, RoutedEventArgs e)
        {
            var alltables = mpHelper.Save_aux(_tables);
            await integration.SaveAllTables(alltables);

            _editing = false;
            Bindings.Update();
        }
        
        private void Wait_List_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(WaitList), null, new DrillInNavigationTransitionInfo());
        }

        private void Light_Tester_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(LightTester), null, new DrillInNavigationTransitionInfo());
        }

        private async void Table_Tapped(object sender, RoutedEventArgs e)
        {

            var rect = (Border)sender;
            var layoutTable = _tables.FirstOrDefault(x => x.Rectangle.Name == rect.Name);

            
            if (_assigning)   
            {
                /*
                var tableId = layoutTable.Table.Id;
                var staff = _staffList.First(x => x.Id == _assigningEmpId);
                var previousBrush = layoutTable.Rectangle.BorderBrush;
                if (staff.Tables == null) { staff.Tables = new List<int>(); }
                if (layoutTable.Table.ServerId != null && layoutTable.Table.ServerId != staff.Id)
                {
                    //layoutTable.Table.Server.assignedTables.Remove(layoutTable.Table); //Remove server from this table
                    layoutTable.Table.ServerId = staff.Id;
                    layoutTable.Table.PreviousServerId = layoutTable.Table.ServerId;
                    layoutTable.Rectangle.BorderBrush = staff.Brush;
                    //staff.assignedTables.Add(layoutTable.Table);
                    staff.Tables.Add(layoutTable.Table.Id);
                }
                else if (layoutTable.Table.ServerId == staff.Id)
                {
                    if (layoutTable.Table.PreviousServerId == null)
                    {
                        layoutTable.Rectangle.BorderBrush = (Brush)ColorHelper.GetSolidColorBrush("FF424242");
                    }
                    else
                    {
                        var previousStaff = _staffList.First(x => x.Id == layoutTable.Table.PreviousServerId);
                        layoutTable.Rectangle.BorderBrush = previousStaff.Brush;
                    }
                    //staff.assignedTables.Remove(layoutTable.Table);
                    staff.Tables.Remove(layoutTable.Table.Id);
                    layoutTable.Table.ServerId = staff.Id;
                    //layoutTable.Table.ServerId  = layoutTable.Table.PreviousServerId;
                }
                else
                {
                    layoutTable.Table.ServerId = staff.Id;
                    layoutTable.Rectangle.BorderBrush = staff.Brush;
                    //staff.assignedTables.Add(layoutTable.Table);
                    staff.Tables.Add(layoutTable.Table.Id);
                }
                */

                var tableId = layoutTable.Table.Id;
                if (_assigningTableIds.Any(x => x == tableId))
                {
                    _assigningTableIds.Remove(tableId);
                    AssignTableBorderColor(layoutTable, _staffList.ToList());
                }
                else
                {
                    rect.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Yellow);
                    _assigningTableIds.Add(tableId);
                }
               
                return;
            }

            if (layoutTable.Table.State == "seated" || layoutTable.Table.State == "holding")
            {
                await ShowOpenTableDialog(rect, layoutTable);
            }

            else if (layoutTable.Table.State == "open")
            {
                ShowSeatTableDialog(rect, layoutTable, sender);
            }
        }

        private async Task AssignTableBorderColor(LayoutTable layoutTable, List<Staff> staffList)
        {
            var staff = staffList.FirstOrDefault(x => x.Tables != null ? x.Tables.Contains(layoutTable.Table.Id) : false);

            var colorToSet = (Brush)ColorHelper.GetSolidColorBrush("FF424242");
            if (staff != null)
            {
               colorToSet = ColorHelper.GetBorderColor(staffList.IndexOf(staff));
                Debug.WriteLine("staffList.IndexOf(staff) " + staffList.IndexOf(staff));

            }

            layoutTable.Rectangle.BorderBrush = colorToSet;
            Debug.WriteLine("layoutTable.Rectangle.BorderBrush " + layoutTable.Rectangle.BorderBrush);

            Bindings.Update();
        }

        private async void ShowSeatTableDialog(Border rect, LayoutTable layoutTable, object sender)
        {
            _seatTableSize = 1;
            _seatTableCapacity = layoutTable.Table.Capacity;
            _seatTableId = layoutTable.Table.Id;
            Bindings.Update();
            SeatTableFlyout.ShowAt((FrameworkElement)sender);
        }
        
        private async Task ShowOpenTableDialog(Border rect, LayoutTable layoutTable)
        {
            var dialog = new ContentDialog // move to dialogues?????
            {
                Content = $"Open Table {layoutTable.Table.Number}?",
                IsSecondaryButtonEnabled = false,
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Primary
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                layoutTable.Table.State = "open";

                await integration.OpenTable(layoutTable.Table.Id);
                var tables = await integration.GetTables();
                await RebuildLayout(tables);
            }
        }

        private async void Seat_Table_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var response = await integration.SitTable(_seatTableId, _seatTableSize);
            if (!string.IsNullOrEmpty(response.LightColor))
            {
                _seatTableId = 0;
                _seatTableSize = 1;
                Bindings.Update();
                var tables = await integration.GetTables();
                await RebuildLayout(tables);
                SeatTableFlyout.Hide();
            }
        }

        private async Task CheckLights(List<Table> tables)
        {
            try
            {
                foreach (var table in tables.Where(x => x.State == "seated"))
                {
                    var seatedMinutes = (DateTime.UtcNow - table.SeatedTime.Value).TotalMinutes;

                    if (seatedMinutes > Settings.LightColorDurationMinutes && seatedMinutes < Settings.LightColorDurationMinutes + 2)
                    {
                        await LightQueueReader.AddItem(new LightQueueCommand { Address = table.LightAddress, TurnOn = false });
                    }
                }
            }
            catch
            {
                await dialogues.ShowErrorDialog("We were unable to set some lights back to normal.");
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                Frame.Navigate(typeof(SettingsPage));
            }
            else
            {
                switch (args.InvokedItem)
                {
                    case "Wait List":
                        Frame.Navigate(typeof(WaitList), null, new DrillInNavigationTransitionInfo());
                        break;
                    case "Staff":
                        Frame.Navigate(typeof(StaffManage), null, new DrillInNavigationTransitionInfo());
                        break;
                    case "Edit Layout":
                        _editing = true;
                        Bindings.Update();
                        break;
                    case "Light Tester":
                        Frame.Navigate(typeof(LightTester), null, new DrillInNavigationTransitionInfo());
                        break;
                }
            }
        }

       private void lv_Loaded(object sender, RoutedEventArgs e)
        {
            var listView = (ListView)sender;
            listView.ItemsSource = _staffList;
        }

        private void AddTable_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateTable), null, new DrillInNavigationTransitionInfo());
        }

        private async void Assign_Tapped(object sender, TappedRoutedEventArgs e) 
        {         
            /*
            foreach (var layoutTable in _tables)
            {
                foreach (var s in _staffList)
                {
                    if (s.Tables == null) { s.Tables = new List<int>(); }
                    if (layoutTable.Table.ServerId != null && layoutTable.Table.ServerId == s.Id)
                    {
                        s.Tables.Add( layoutTable.Table.Server.Id);
                        break;
                    }
                }                
            } 
            */
            
            foreach (var staff in _staffList)
            {
                if (staff.Id == _assigningEmpId)
                {
                    if(staff.Tables == null)
                    {
                        staff.Tables = new List<int>();
                    } 
                    staff.Tables.AddRange(_assigningTableIds.Where(x => !staff.Tables.Contains(x)));
                }
                else
                {
                    if(staff.Tables != null && staff.Tables.Any())
                    {
                        staff.Tables.RemoveAll(x => _assigningTableIds.Contains(x));
                    }
                }
            }

            var result = await integration.SaveStaffSections(_staffList.ToList());
            if(result)
            {
                _assigning = false;
                _assigningEmpId = 0;
                _assigningTableIds = new List<int>();
                await GetRestaurant();
                var tables = await integration.GetTables();
                RebuildLayout(tables);
                Bindings.Update();
            }
        }

        private async void AssignTables_Invoked(object sender, TappedRoutedEventArgs args)
        {
            var param = sender as Button;
            var staff = (Staff)param.DataContext;
            if (staff == null)
            {
                return;
            }
            _assigningTableIds = new List<int>();
            _assigning = true;
            _assigningEmpId = staff.Id;
            Bindings.Update();
        }

        /* //Moved to StaffManage.cs
        private async void ClockOut_Invoked(object sender, TappedRoutedEventArgs args)
        { ......... }
        */
        private async void Suggest_Table(object sender, TappedRoutedEventArgs args)
        {
            if (_sectionList == null || !_sectionList.Any())
            {
                _sectionList = mpHelper.CreateSectionList(_staffList, _tables);
            }
            var table = mpHelper.doRoundRobinAssignment(ref _sectionList);
            if (table.Number == null)
            {
                dialogues.ShowErrorDialog("All tables are occupied");
            }
            else
            {
                table.SeatedTime = DateTime.UtcNow; //This is a temp solution, it doesnt solve possible problems. Need to work with the API and the database
                table.State = "seated";
            }

            var sitResponse = await integration.SitTable(table.Id, 1); //Hard coded a "1"

            var tables = await integration.GetTables();
            RebuildLayout(tables);
            Bindings.Update();
        }
    }
}
