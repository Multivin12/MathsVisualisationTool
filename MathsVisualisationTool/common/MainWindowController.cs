﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MathsVisualisationTool
{

    public partial class MainWindow : Window
    {
        public string InputBoxValue
        {
            get { return inputBox.Text; }
            set { inputBox.Text = value; }
        }

        //Fields for the MainWindow Class.
        LiveChartsDrawer l;
        GraphDrawer c;
        //Stop Watch Variables.
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        string currentTime = string.Empty;

        /// <summary>
        /// Constructor for the MainWindow object.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            SourceInitialized += (s, e) =>
            {
                IntPtr handle = (new WindowInteropHelper(this)).Handle;
                HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
            };

            inputBox.KeyDown += new KeyEventHandler(InputBox_KeyDown);

            /**************************************** DATAGRID FUNCTIONS ************************************/
            #region DataGrid
            //To load the variables into the datagrid.
            loadVarsIntoDataGrid();
            var column = new DataGridTextColumn();
            #endregion
            
            /**************************************** CLOCK FUNCTIONS ***************************************/
            #region Clock
            dt.Tick += new EventHandler(MainStopwatch);
            dt.Tick += new EventHandler(MainClock);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            //dt.Interval = TimeSpan.FromSeconds(1);
            dt.Start();
            #endregion
            
            /************************************* LIVE CHART FUNCTIONS *************************************/
            #region LiveCharts
            l = new LiveChartsDrawer(this);
            l.Draw();
            this.DataContext = l;

            //////////////////////////////////////////////////////////////////////////////////////////////////
            ///To change the colour of the tooltip - LvChrt is an object pointing to the LiveCharts container.
            ///https://lvcharts.net/App/examples/v1/wf/Tooltips%20and%20Legends
            LvChrt.DataTooltip.Background = Brushes.Black;
            #endregion
        }

        /*
         * OnNewNote_Clicked - Handle event if Add New Note button is 
         *                  clicked from the Notes Popout Pane.
         */
        private void OnNewNote_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add New Note - Fix IT!!!!");
        }

        /****************************************TO WORK ON************************************************************/


        private void CommonCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /****************************************************************************************************/

        /************************ CLOCK FUNCTIONS (INC. CLOCK/TIMER/STOPWATCH/CALENDAR) *********************/
        #region ClockFunctions

        void MainStopwatch(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                sWatchLabel.Content = currentTime;
            }
        }

        /*
         * MainClock -  Handle event for the digital clock feature
         *              which displays current time
         */
        void MainClock(object sender, EventArgs e)
        {
            clockTime.Content = DateTime.Now.ToLongTimeString();
            currentDay.Content = DateTime.Now.DayOfWeek;
            dateTime.Content = DateTime.Now.ToLongDateString();
        }

        /*
         * ResetStopwatch_Clicked - Handle event if Starts Stopwatch button is 
         *                          clicked, starts the stopwatch running
         */
        private void StartStopwatch_Clicked(object sender, EventArgs e)
        {
            stopWatch.Start();
        }

        /*
         * StopStopwatch_Clicked - Handle event if Stop Stopwatch button is 
         *                          clicked, stops the stopwatch if running
         */
        private void StopStopwatch_Clicked(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
                stopWatch.Stop();
        }

        /*
         * LapStopwatch_Clicked - Handle event if Lap Stopwatch button is 
         *                        clicked, records split/lap time to list
         */
        private void LapStopwatch_Clicked(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
                lapTimes_List.Items.Add(currentTime);
        }

        /*
         * ResetStopwatch_Clicked - Handle event if Reset Stopwatch button is 
         *                          clicked, resets the stopwatch to zero
         */
        private void ResetStopwatch_Clicked(object sender, EventArgs e)
        {
            stopWatch.Reset();
            sWatchLabel.Content = "00:00:00:00";
        }

        /*
         * ClearLapStopwatch_Clicked -  Handle event if Clear button is 
         *                              clicked, clears all stored lap 
         *                              times
         */
        private void ClearLapStopwatch_Clicked(object sender, EventArgs e)
        {
            lapTimes_List.Items.Clear();
        }

        private int _timerHour = 0;
        public int timerHourValue
        {
            get { return _timerHour; }
            set { _timerHour = value; hoursLabel.Content = value.ToString(); }
        }

        /*
         * OnHourUp_Clicked -   Handle event if Increase Hour button is 
         *                      clicked, increases the hour label by 1 
         *                      per click
         */
        private void OnHourUp_Clicked(object sender, EventArgs e)
        {
            hoursLabel.Content = timerHourValue++;
        }


        /*
         * OnHourDown_Clicked -   Handle event if decreases Hour button is 
         *                      clicked, decreases the hour label by 1 
         *                      per click
         */
        private void OnHourDown_Clicked(object sender, EventArgs e)
        {
            hoursLabel.Content = timerHourValue--;
        }

        /*
         * OnMinUp_Clicked -    Handle event if Increase Minute button is 
         *                      clicked, increases the minute label by 1 
         *                      per click
         */
        private void OnMinUp_Clicked(object sender, EventArgs e)
        {

        }

        /*
         * OnMinDown_Clicked -   Handle event if decreases Min button is 
         *                      clicked, decreases the minute label by 1 
         *                      per click
         */
        private void OnMinDown_Clicked(object sender, EventArgs e)
        {

        }

        /*
         * OnSecUp_Clicked -   Handle event if Increase Second button is 
         *                      clicked, increases the seconds label by 1 
         *                      per click
         */
        private void OnSecUp_Clicked(object sender, EventArgs e)
        {

        }

        /*
         * OnSecDown_Clicked -   Handle event if decreases Sec button is 
         *                      clicked, decreases the sec label by 1 
         *                      per click
         */
        private void OnSecDown_Clicked(object sender, EventArgs e)
        {

        }
        #endregion
        
        /****************************** DRAG/DROP FUNCTIONS [NONE FUNCTIONAL ATM] ***************************/
        #region DragDropFunctions
        private void OnDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Button draggedBtn)
            {
                DragDrop.DoDragDrop(draggedBtn, draggedBtn, DragDropEffects.Copy);
            }
        }

        void onDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(e.Data.GetFormats()[0]) is Button droppedBtn)
            {
                functionPanel.Children.Remove(droppedBtn);
                favsPanel.Children.Add(droppedBtn);
            }
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            IDataObject draggedData = NewMethod(e);
            if (draggedData.GetData(draggedData.GetFormats()[0]) is Button droppedBtn)
            {
                functionPanel.Children.Remove(droppedBtn);
                favsPanel.Children.Add(droppedBtn);
            }
        }

        private static IDataObject NewMethod(DragEventArgs e)
        {
            return e.Data;
        }
        #endregion

        /************************************ FUNCTIONS TO RUN/SUBMIT INPUT *********************************/
        #region RunSubmitFunctions
        /*
         * OnRun_Clicked - Handle event if the Run/Submit button is 
         *                  clicked.
         */
        private void OnRun_Clicked(object sender, RoutedEventArgs e)
        {
            HandleTextEnter();
        }

        /*
         * InputBox_KeyDown - Handle event if the return button has 
         *                  been pressed.
         */
        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                e.Handled = true;
                HandleTextEnter();
            }
        }

        /*
         * HandleTextEnter - Handle the text enter and put the contents 
         *                  of the text box into the interpreter.
         */
        private void HandleTextEnter()
        {
            Results.Items.Add(">>> \t" + this.inputBox.Text);
            //Interpreter String called results
            Interpreter i = new Interpreter(ref l);
            try
            {
                Results.Items.Add(i.RunInterpreter(inputBox.Text));
            }
            catch (Exception exp)
            {
                if (exp is SolveItException)
                {
                    SolveItException s = (SolveItException) exp;
                    ErrorMsg e = new ErrorMsg(s.Message, s.ErrorCode);
                    e.ShowDialog();
                    Results.Items.Add("Error Code - " + s.ErrorCode);
                }
                else
                {
                    //This shouldn't happen but cannot always pickup all bugs!
                    UnknownErrorException u = new UnknownErrorException(exp.Message);
                    ErrorMsg e = new ErrorMsg(u.Message, u.ErrorCode);
                    e.ShowDialog();
                    Results.Items.Add("Error Code - " + u.ErrorCode);
                }
                Console.WriteLine(exp.ToString());
            }
            var scrollViewer = GetDescendantByType(Results, typeof(ScrollViewer)) as ScrollViewer;
            scrollViewer.ScrollToBottom();
            this.inputBox.Focus();
            this.inputBox.Clear();
            loadVarsIntoDataGrid();
        }

        /// <summary>
        /// Function to extract the ScrollViewer from the listBox.
        /// Code Referenced from Stackoverflow.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null)
            {
                return null;
            }
            if (element.GetType() == type)
            {
                return element;
            }
            Visual foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }
        #endregion

        /****************************************** ALL SAVE FUNCTIONS **************************************/
        #region SaveFunctions

        /* 
         *  FOLLOWING REGION CONTAINS ALL FUNCTIONS RELATING TO SOLVEIT'S SAVE FEATURES/FUNCTIONS
         *  Put in their own region since save buttons are situated both in the toolbar and the
         *  main menu sections
         */

        /*
         * OnSave_Clicked - Handle event if the Save button is 
         *                  click from the toolbar. Saves the
         *                  main numerical list box results
         *                  straight to a text file
         */
        private void OnSave_Clicked(object sender, RoutedEventArgs e)
        {
            Saver.saveWorkshop(this);
        }

        /*
         * OnSaveAll_Clicked -  Handle event if the Save All button is 
         *                      click from the toolbar. Used to save 
         *                      multiple aspects of SolveIT i.e. the
         *                      variable table, numerical output etc.
         */
        private void OnSaveAll_Clicked(object sender, RoutedEventArgs e)
        {
            Saver.saveAll(this);
        }

        /*
         * OnSaveVar_Clicked -  Handle event if the Save Variable button 
         *                      is clicked. Used to save the variable
         *                      table.
         */
        private void OnSaveVar_Clicked(object sender, RoutedEventArgs e)
        {
            Saver.saveVariablesIntoExternalFile();
        }

        /*
         * OnSaveCanvas_Clicked -   Handle event if the Save Canvas button 
         *                          is clicked. Used to save the canvas
         *                          graph as a PNG image.
         */
        private void OnSaveCanvas_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Saver.saveCanvasGraphOntoExternalFile(this);
            } catch (Exception err)
            {
                UnknownErrorException u = new UnknownErrorException("An unknown error has occured - make sure that the canvas graph tab has been rendered before saving and that the directory given is correct.");
                ErrorMsg e2 = new ErrorMsg(u.Message, u.ErrorCode);
                e2.ShowDialog();
            }
        }

        /*
         * OnSaveLVC_Clicked -  Handle event if the Save Canvas button 
         *                      is clicked. Used to save the Live Charts
         *                      graph as an image.
         */
        private void OnSaveLVC_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Saver.saveLiveChartsToExternalFile(this);
            } catch(Exception err)
            {
                UnknownErrorException u = new UnknownErrorException("An unknown error has occured - make sure that the live charts tab has been rendered before saving and that the directory given is correct.");
                ErrorMsg e2 = new ErrorMsg(u.Message, u.ErrorCode);
                e2.ShowDialog();
            }
        }

        #endregion

        /************************************** STANDARD TOP MENU FUNCTIONS *********************************/
        #region TopMenuFunctions

        /*
         * OnGoToWorkshop_Clicked - Handle event if the Go To menuitem is 
         *                          clicked from the standard view Menu.
         */
        private void OnGoToWorkshop_Clicked(object sender, RoutedEventArgs e)
        {
            outputTabCon.SelectedIndex = 0;
        }

        /*
         * OnGoToCanvas_Clicked - Handle event if the Go To menuitem is 
         *                          clicked from the standard view Menu.
         */
        private void OnGoToCanvas_Clicked(object sender, RoutedEventArgs e)
        {
            outputTabCon.SelectedIndex = 1;
        }

        /*
         * OnGoToLVC_Clicked - Handle event if the Go To menuitem is 
         *                          clicked from the standard view Menu.
         */
        private void OnGoToLVC_Clicked(object sender, RoutedEventArgs e)
        {
            outputTabCon.SelectedIndex = 2;
        }

        /*
         * OnExitMenuClicked - Handle event if the Exit button is 
         *                  clicked from the standard File Menu.
         */
        private void OnExitMenu_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult exitMsg = MessageBox.Show("Are you sure you wish to leave?", "Caution", MessageBoxButton.YesNo);
            switch (exitMsg)
            {
                case MessageBoxResult.Yes:
                    Environment.Exit(0);
                    break;
                case MessageBoxResult.No:
                    break;          
            }
        }

        /*
         * OnExitMenuClicked - Handle event if the Exit button is 
         *                  clicked from the standard File Menu.
         */
        private void OnClose_Clicked(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        /*
         * OnMaximise_Clicked - Handle event if the Resize button is 
         *                  clicked from the View Menu or title bar.
         */
        private void OnMaximise_Clicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }
        /*
         * OnResize_Clicked - Handle event if the Resize button is 
         *                  clicked from the View Menu or title bar.
         */
        private void OnResize_Clicked(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        /*
         * OnMinimise_Clicked - Handle event if the Minimise button is 
         *                      clicked from the View Menu or title bar.
         */
        private void OnMinimise_Clicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /*
         * OnVariables_Clicked - Basic Inputs Documentation
         */
        private void OnVariables_Clicked(object sender, RoutedEventArgs e)
        {
            XpsDocument testDocument = new XpsDocument("../../manuals/documentation/variables.xps", FileAccess.Read);
            mainDocViewer.Document = testDocument.GetFixedDocumentSequence();
        }

        /*
         * OnBasicInput_Clicked - Basic Inputs Documentation
         */
        private void OnBasicInput_Clicked(object sender, RoutedEventArgs e)
        {
            XpsDocument testDocument = new XpsDocument("../../manuals/documentation/basicInput.xps", FileAccess.Read);
            mainDocViewer.Document = testDocument.GetFixedDocumentSequence();
        }

        /*
         * OnInputDoc_Clicked - Maths Commands Documentation
         */
        private void OnInputDoc_Clicked(object sender, RoutedEventArgs e)
        {
            XpsDocument testDocument = new XpsDocument("../../manuals/documentation/MathsCommands.xps", FileAccess.Read);
            mainDocViewer.Document = testDocument.GetFixedDocumentSequence();
        }

        /*
         * OnErrorCodes_Clicked - Error Codes Documentation
         */
        private void OnErrorCodes_Clicked(object sender, RoutedEventArgs e)
        {
            XpsDocument testDocument = new XpsDocument("../../manuals/documentation/ErrorCodes.xps", FileAccess.Read);
            mainDocViewer.Document = testDocument.GetFixedDocumentSequence();
        }
        #endregion

        /**************************************** TOOLBAR MENU FUNCTIONS ************************************/
        #region ToolBarFunctions

        /*
         * OnSearch_Clicked -   Handle event if the Search button is 
         *                      click from the toolbar, searchs output
         *                      listbox
         */
        private void OnSearch_Clicked(object sender, EventArgs e)
        {
            //Results.SelectedItems.Clear();
            //for(int i = Results.Items.Count - 1; i>= 0;i--)
            //{
            //    if(Results.Items[i].ToString().ToLower().Contains(searchBox.Text.ToLower()))
            //    {
            //        Results.SetSelected(i,true);
            //    }
            //}
            ///*label*/.Text=Results.SelectedItems.Count.ToString() + " items found"
        }

        /*
         * OnUndo_Clicked -  Handle event if the Undo button is 
         *                      click from the toolbar
         */
        private void OnUndo_Clicked(object sender, RoutedEventArgs e)
        {
            // MADE FUNCTIONAL THROUGH XAML COMMAND / COMMAND BINDIN
        }

        /*
         * OnRedo_Clicked -  Handle event if the Redo button is 
         *                  click from the toolbar
         */
        private void OnRedo_Clicked(object sender, RoutedEventArgs e)
        {
            // MADE FUNCTIONAL THROUGH XAML COMMAND / COMMAND BINDIN
        }

        /*
         * OnForward_Clicked -  Handle event if the Forward button is 
         *                      click from the toolbar
         */
        private void OnForward_Clicked(object sender, RoutedEventArgs e)
        {
            int newIndex = outputTabCon.SelectedIndex + 1;
            outputTabCon.SelectedIndex = newIndex % 3;
        }

        /*
         * OnBack_Clicked -  Handle event if the Back button is 
         *                  click from the toolbar
         */
        private void OnBack_Clicked(object sender, RoutedEventArgs e)
        {
            int newIndex = outputTabCon.SelectedIndex - 1;
            if(newIndex < 0)
            {
                newIndex = 2;
            }
            outputTabCon.SelectedIndex = newIndex % 3;
        }

        /*
         * OnNew_Clicked -  Handle event if the New button is 
         *                  click from the toolbar
         */
        private void OnNew_Clicked(object sender, RoutedEventArgs e)
        {
            Results.Items.Clear();
        }

        /*
         * OnOpenVar_Clicked -  Handle event if the Open button is 
         *                      click from the toolbar or menu - 
         *                      opens variables
         */
        private void OnOpenVar_Clicked(object sender, RoutedEventArgs e)
        {
            Loader.loadVarFileIntoDataGrid(this);
        }

        /*
         * OnOpenNum_Clicked -  Handle event if the Open button is 
         *                      click from the toolbar or menu - 
         *                      opens numerical workshop
         */
        private void OnOpenNum_Clicked(object sender, RoutedEventArgs e)
        {
            Loader.loadTextFileIntoNumericalWorkshop(this);
        }

        /*
         * OnPrint_Clicked - Handle event if the Print button is 
         *                  click from the toolbar
         */
        private void OnPrint_Clicked(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintVisual(Results, "Printing Results...");
        }

        /*
         * OnPageSetup_Clicked - Handle event if the Page Setup button is 
         *                      click from the toolbar
         */
        private void OnPageSetup_Clicked(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            var dialogResult = printDialog.ShowDialog();

            if(dialogResult == true)
            {

            }
        }

        /*
         * OnCut_Clicked -  Handle event if the Cut button is 
         *                  click from the toolbar
         */
        private void OnCut_Clicked(object sender, RoutedEventArgs e)
        {
            // MADE FUNCTIONAL THROUGH XAML COMMAND / COMMAND BINDING
        }

        /*
         * OnCopy_Clicked - Handle event if the Copy button is 
         *                  click from the toolbar
         */
        private void OnCopy_Clicked(object sender, RoutedEventArgs e)
        {
            // MADE FUNCTIONAL THROUGH XAML COMMAND /COMMAND BINDING
        }

        /*
         * OnPaste_Clicked - Handle event if the Paste button is 
         *                   click from the toolbar
         */
        private void OnPaste_Clicked(object sender, RoutedEventArgs e)
        {
            // MADE FUNCTIONAL THROUGH XAML COMMAND / COMMAND BINDING
        }

        /*
         * OnSettings_Clicked - Handle event if the Settings button is 
         *                  click from the toolbar
         */
        private void OnSettings_Clicked(object sender, RoutedEventArgs e)
        {
            Settings appSettings = new Settings();
            appSettings.Show();
        }

        /*
         * OnHelp_Clicked - Handle event if the Help button is 
         *                   click from the toolbar
         */
        private void OnHelp_Clicked(object sender, RoutedEventArgs e)
        {
            HelpLibrary library = new HelpLibrary();
            library.Show();
        }

        /*
         * OnKeypad_Clicked - Handle event if the Keypad button is 
         *                   click from the toolbar
         */
        private void OnKeypad_Clicked(object sender, RoutedEventArgs e)
        {
            KeyPad keypad = new KeyPad();
            keypad.Show();
        }

        /*
         * OnGraph_Clicked - Handle event if the Graph button is 
         *                   click from the toolbar
         */
        private void OnGraph_Clicked(object sender, RoutedEventArgs e)
        {
            if (graphComboBox.Text == "Canvas Graph")
                outputTabCon.SelectedIndex = 1;
            else if (graphComboBox.Text == "Live Charts")
                outputTabCon.SelectedIndex = 2;
            else
                MessageBox.Show("Please use Drop Down Menu to select Chart Type");
        }

        /*
         * OnUpdate_Clicked - Handle event if the update button is 
         *                  click from the toolbar
         */
        private void OnUpdate_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You are up to date");
        }

        /*
         * OnTrash_Clicked - Handle event if the trash button is 
         *                  click from the toolbar
         */
        private void OnTrash_Clicked(object sender, RoutedEventArgs e)
        {
            Results.SelectionMode = SelectionMode.Multiple;

            for (int i = Results.SelectedItems.Count - 1; i >= 0; i--)
            {
                Results.Items.Remove(Results.SelectedItems[i]);
            }
        }

        #endregion

        /********************************* GREEK CHARACTERS KEYPAD FUNCTIONS ********************************/
        #region GreekCharacters 

        /*
         * onAlpha_Clicked -    Function for the Alpha Character Button on the
         *                      keypad in the right side of the Main Window
         *                      Dock Panel - NOTE: Character created with
         *                      Unicode Escape Characters/Code
         */
        private void OnAlpha_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03B1";
        }

        /*
         * onBeta_Clicked - Function for the Beta Character Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character created with
         *                  Unicode Escape Characters/Code
         */
        private void OnBeta_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03B2";
        }

        /*
         * onDelta_Clicked -    Function for the Delta Button Button on the
         *                      keypad in the right side of the Main Window
         *                      Dock Panel - NOTE: Character created with
         *                      Unicode Escape Characters/Code
         */
        private void OnDelta_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u0394";
        }

        /*
         * on_delta_Clicked -   Function for the lower case delta Character Button
         *                      on the keypad in the right side of the Main Window
         *                      Dock Panel - NOTE: Character created with
         *                      Unicode Escape Characters/Code
         */
        private void On_delta_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03B4";
        }

        /*
         * onEpsilon_Clicked -  Function for the Epsilon Character Button on the
         *                      keypad in the right side of the Main Window
         *                      Dock Panel - NOTE: Character created with
         *                      Unicode Escape Characters/Code
         */
        private void OnEpsilon_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03B5";
        }

        /*
         * onGamma_Clicked -    Function for the Gamma Button Button on the
         *                      keypad in the right side of the Main Window
         *                      Dock Panel - NOTE: Character created with
         *                      Unicode Escape Characters/Code
         */
        private void OnGamma_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03B3";
        }

        /*
         * onLambda_Clicked -   Function for the Lambda Character Button on the
         *                      keypad in the right side of the Main Window
         *                      Dock Panel - NOTE: Character created with
         *                      Unicode Escape Characters/Code
         */
        private void OnLambda_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03BB";
        }

        /*
         * onTheta_Clicked -    Function for the Theta Character Button on the
         *                      keypad in the right side of the Main Window
         *                      Dock Panel - NOTE: Character created with
         *                      Unicode Escape Characters/Code
         */
        private void OnTheta_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03B8";
        }

        /*
         * onMu_Clicked -   Function for the Mu Button Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character created with
         *                  Unicode Escape Characters/Code
         */
        private void OnMu_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03BC";
        }

        /*
         * onOmega_Clicked -    Function for the Omega Character Button on the
         *                      keypad in the right side of the Main Window
         *                      Dock Panel - NOTE: Character created with
         *                      Unicode Escape Characters/Code
         */
        private void OnOmega_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03A9";
        }

        /*
         * on_omega_Clicked -   Function for the Lower case Omega Character Button
         *                      on the keypad in the right side of the Main Window
         *                      Dock Panel - NOTE: Character created with
         *                      Unicode Escape Characters/Code
         */
        private void On_omega_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03C9";
        }

        /*
         * onPhi_Clicked -  Function for the Phi Button Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character created with
         *                  Unicode Escape Characters/Code
         */
        private void OnPhi_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03C6";
        }

        /*
         * onPsi_Clicked -  Function for the Psi Character Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character created with
         *                  Unicode Escape Characters/Code         */
        private void OnPsi_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03C8";
        }

        /*
         * onRho_Clicked -  Function for the Rho Character Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character created with
         *                  Unicode Escape Characters/Code
         */
        private void OnRho_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03C1";
        }

        /*
         * onSigma_Clicked - Function for the Sigma Button Button on the
         *                   keypad in the right side of the Main Window
         *                   Dock Panel - NOTE: Character created with
         *                   Unicode Escape Characters/Code
         */
        private void OnSigma_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            this.inputBox.Text += "\u03A3";
        }

        #endregion

        /************************** ALGEBRA/MATHEMATICAL FUNCTIONS KEYPAD FUNCTIONS *************************/
        #region AlgebraFunctions
        /*
         * OnModulus_Clicked -   Function for the Modulus Button on the
         *                          keypad in the right side of the Main Window
         *                          Dock Panel - NOTE: Character can be created
         *                          with Unicode Escape Characters/Code
         */
        private void OnModulus_Clicked(object sender, RoutedEventArgs e)
        {
            // NEED TO THINK ABOUT THIS
            this.inputBox.Text += "abs()";
        }

        /*
         * onLeftBracketClicked -   Function for the Left Bracket Button on the
         *                          keypad in the right side of the Main Window
         *                          Dock Panel - NOTE: Character can be created
         *                          with Unicode Escape Characters/Code
         */
        private void OnLeftBracket_Clicked(object sender, RoutedEventArgs e)
        {
            // \u0028 => "(" => Left/Opening Parenthesis
            this.inputBox.Text += "(";
        }

        /*
         * onRightBracketClicked -  Function for the Right Bracket Button on the
         *                          keypad in the right side of the Main Window
         *                          Dock Panel - NOTE: Character can be created
         *                          with Unicode Escape Characters/Code
         */
        private void OnRightBracket_Clicked(object sender, RoutedEventArgs e)
        {
            // \u0029 => ")" => Right/Closing Parenthesis
            this.inputBox.Text += ")";
        }

        /*
         * OnLessEqual_Clicked -   Function for the Less Than or Equal to Button on the
         *                          keypad in the right side of the Main Window
         *                          Dock Panel - NOTE: Character can be created
         *                          with Unicode Escape Characters/Code
         */
        private void OnLessEqual_Clicked(object sender, RoutedEventArgs e)
        {
            // \u0028 => "(" => Left/Opening Parenthesis
            this.inputBox.Text += "<";
        }

        /*
         * OnMoreEqual_Clicked -  Function for the Greater than or Equal to Button on the
         *                          keypad in the right side of the Main Window
         *                          Dock Panel - NOTE: Character can be created
         *                          with Unicode Escape Characters/Code
         */
        private void OnMoreEqual_Clicked(object sender, RoutedEventArgs e)
        {
            // \u0029 => ")" => Right/Closing Parenthesis
            this.inputBox.Text += ">";
        }

        /*
         * onIndiceClicked -    Function for the Indice Button on the
         *                      keypad in the right side of the Main Window
         *                      Dock Panel
         */
        private void OnIndice_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "^";
        }

        /*
         * onMulClicked -   Function for the Multiplication Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character can be created
         *                  with Unicode Escape Characters/Code
         */
        private void OnMul_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            // \u00D7 => \times => x
            // \u00B7 => \cdot => . (but in the middle of the line!)
            this.inputBox.Text += "\u00D7";
        }

        /*
         * onDivClicked -   Function for the Division Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character created with
         *                  Unicode Escape Characters/Code
         */
        private void OnDiv_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            // \u00F7 => Division Sign
            // \u2215 => Division Slash => /
            this.inputBox.Text += "\u00F7";
        }

        /*
         * onFracClicked -  Function for the Fraction Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character can be created
         *                  with Unicode Escape Characters/Code
         */
        private void OnFrac_Clicked(object sender, RoutedEventArgs e)
        {
            // Need to think about symbol since it clashes with above
            // OnDivClicked
            // \u2044 => Fraction Slash (Unicode Charatcer)
            this.inputBox.Text += "/";
        }

        /*
         * onLessThanClicked -      Function for the Less than Button on the
         *                          keypad in the right side of the Main Window
         *                          Dock Panel - NOTE: Character can be created
         *                          with Unicode Escape Characters/Code
         */
        private void OnLessThan_Clicked(object sender, RoutedEventArgs e)
        {
            // \u0028 => "(" => Left/Opening Parenthesis
            this.inputBox.Text += "<";
        }



        /*
         * onGreaterThanClicked -   Function for the Greater than Button on the
         *                          keypad in the right side of the Main Window
         *                          Dock Panel - NOTE: Character can be created
         *                          with Unicode Escape Characters/Code
         */
        private void OnGreaterThan_Clicked(object sender, RoutedEventArgs e)
        {
            // \u0029 => ")" => Right/Closing Parenthesis
            this.inputBox.Text += ">";
        }

        /*
         * onSqrtClicked -  Function for the Square Root Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character can be created
         *                  with Unicode Escape Characters/Code
         */
        private void OnSqrt_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Characters/Code to render special Characters
            // \u221A => Square Root
            // \u221B => Cube Root
            // \u221C => Fourth Root
            this.inputBox.Text += "sqrt()";
        }

        /*
         * onLogClicked -  Function for the Log Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel
         */
        private void OnLog_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "log()";
        }

        /*
         * onLnClicked -    Function for the ln Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character can be created
         *                  with Unicode Escape Characters/Code
         */
        private void OnLn_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Characters/Code to render special Characters
            this.inputBox.Text += "ln()";
        }

        /*
         * OnEuler_Clicked - Function for the Euler's E Button on the
         *                   keypad in the right side of the Main Window
         *                   Dock Panel - NOTE: Character can be created
         *                   with Unicode Escape Characters/Code
         */
        private void OnEuler_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "\uD835\uDC52";
        }

        /*
         * onSinClicked -   Function for the Sin Button on the keypad
         *                  in the right side of the Main Window
         *                  Dock Panel - NOTE: Character can be created
         *                  with Unicode Escape Characters/Code
         */
        private void OnSin_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "sin()";
        }

        /*
         * onCosClicked -   Function for the Cos Button on the keypad
         *                  in the right side of the Main Window
         *                  Dock Panel - NOTE: Character can be created
         *                  with Unicode Escape Characters/Code
         */
        private void OnCos_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "cos()";
        }

        /*
         * onTanClicked -   Function for the Tan Button on the keypad
         *                  in the right side of the Main Window
         *                  Dock Panel - NOTE: Character can be created
         *                  with Unicode Escape Characters/Code
         */
        private void OnTan_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "tan()";
        }

        #endregion

        /************************************** NUMERICAL KEYPAD FUNCTIONS **********************************/
        #region Numerical
        /*
         * onEqualClicked -    Function for the equals Button on the
         *                     keypad in the right side of the Main Window
         *                     Dock Panel - NOTE: Character can be created
         *                     with Unicode Escape Characters/Code
         */
        private void OnEqual_Clicked(object sender, RoutedEventArgs e)
        {
            // \u0028 => "(" => Left/Opening Parenthesis
            this.inputBox.Text += "=";
        }

        /*
         * onAddClicked -   Function for the Addition Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character can be created
         *                  with Unicode Escape Characters/Code
         */
        private void OnAdd_Clicked(object sender, RoutedEventArgs e)
        {
            // Use Unicode Escape Code/Characters
            // \u002B => + (Unicode Character)
            this.inputBox.Text += "+";
        }

        /*
         * onSubClicked -   Function for the Subtraction Button on the
         *                  keypad in the right side of the Main Window
         *                  Dock Panel - NOTE: Character can be created
         *                  with Unicode Escape Characters/Code
         */
        private void OnSub_Clicked(object sender, RoutedEventArgs e)
        {
            // \u002D => Hypen-Minus in Unicode
            this.inputBox.Text += "-";
        }

        /*
         * onDecimalClicked -   Function for the Decimal Button on the
         *                      keypad in the right side of the Main
         *                      Window Dock Panel
         */
        private void OnDecimal_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += ".";
        }

        /*
         * onAnsClicked -   Function for the Ans Button on the
         *                  keypad in the right side of the Main
         *                  Window Dock Panel
         */
        private void OnPi_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "\u03C0";
        }

        /*
         * on0_Clicked -    Function for the 0 Button on the
         *                  keypad in the right side of the Main
         *                  Window Dock Panel
         */
        private void On0_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "0";
        }

        /*
         * on1_Clicked -    Function for the 1 Button on the
         *                  keypad in the right side of the Main
         *                  Window Dock Panel
         */
        private void On1_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "1";
        }

        /*
         * on2_Clicked -    Function for the 2 Button on the
         *                  keypad in the right side of the Main
         *                  Window Dock Panel
         */
        private void On2_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "2";
        }

        /*
         * on3_Clicked -    Function for the 3 Button on the
         *                  keypad in the right side of the Main
         *                  Window Dock Panel
         */
        private void On3_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "3";
        }

        /*
         * on4_Clicked -    Function for the 4 Button on the
         *                  keypad in the right side of the Main
         *                  Window Dock Panel
         */
        private void On4_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "4";
        }

        /*
         * on5_Clicked -    Function for the 5 Button on the
         *                  keypad in the right side of the Main
         *                  Window Dock Panel
         */
        private void On5_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "5";
        }

        /*
         * on6_Clicked -    Function for the 6 Button on the
         *                  keypad in the right side of the Main
         *                  Window Dock Panel
         */
        private void On6_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "6";
        }

        /*
         * on7_Clicked -    Function for the 7 Button on the
         *                  keypad in the right side of the Main
         *                  Window Dock Panel
         */
        private void On7_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "7";
        }

        /*
         * on8_Clicked -    Function for the 8 Button on the
         *                  keypad in the right side of the Main
         *                  Window Dock Panel
         */
        private void On8_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "8";
        }

        /*
         * on9_Clicked -    Function for the 9 Button on the
         *                  keypad in the right side of the Main
         *                  Window Dock Panel
         */
        private void On9_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "9";
        }
        #endregion

        /*************************************** VARIABLE TABLE FUNCTIONS***********************************/
        #region VariableTable

        public class VariableDetails
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// Method to load in the variables and store them into the datagrid.
        /// </summary>
        public void loadVarsIntoDataGrid()
        {
            //Get the variables and put them into a hashtable
            Hashtable vars = VariableFileHandle.getVariables();
            //Sort the hashtable as the vars should be in alphabetical order.
            SortedDictionary<string, string> sortedVars = new SortedDictionary<string, string>();
            foreach(DictionaryEntry d in vars)
            {
                string key = (string)d.Key;
                string value = (string)d.Value;
                //Don't add the variables with the '~' symbol infront as these are not user-defined variables.
                if (key.Contains("~"))
                {
                    continue;
                }
                else
                {
                    sortedVars.Add((string)d.Key,(string)d.Value);
                }
            }

            //To add something to the datagrid it must be an ObservableCollection object.
            ObservableCollection<VariableDetails> varInfoToAdd = new ObservableCollection<VariableDetails>();
            //Add the variables to the ObservableCollection object.
            foreach(var d in sortedVars)
            {
                varInfoToAdd.Add(new VariableDetails { Name = d.Key, Value = d.Value });
            }
            //Set the itemSource of the datagrid to the ObservableCollection.
            varTable.ItemsSource = varInfoToAdd;
        }
        #endregion
        
        /*************************************** GRAPH KEYPAD FUNCTIONS ************************************/
        #region GraphKeypad

        /*
         * OnPlot_Clicked -    Function for the plot Button on the
         *                     keypad in the right side of the Main Window
         *                     Dock Panel - NOTE: Character can be created
         *                     with Unicode Escape Characters/Code
         */
        private void OnPlot_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "plot";
        }

        /*
         * OnComma_Clicked -    Function for the Comma Button on the
         *                     keypad in the right side of the Main Window
         *                     Dock Panel - NOTE: Character can be created
         *                     with Unicode Escape Characters/Code
         */
        private void OnComma_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += ",";
        }

        /*
         * OnX_Clicked -    Function for the x Button on the
         *                     keypad in the right side of the Main Window
         *                     Dock Panel - NOTE: Character can be created
         *                     with Unicode Escape Characters/Code
         */
        private void OnX_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "x";
        }

        /*
         * OnY_Clicked -    Function for the y Button on the
         *                     keypad in the right side of the Main Window
         *                     Dock Panel - NOTE: Character can be created
         *                     with Unicode Escape Characters/Code
         */
        private void OnY_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "y";
        }

        /*
         * OnC_Clicked -    Function for the c Button on the
         *                     keypad in the right side of the Main Window
         *                     Dock Panel - NOTE: Character can be created
         *                     with Unicode Escape Characters/Code
         */
        private void OnC_Clicked(object sender, RoutedEventArgs e)
        {
            this.inputBox.Text += "c";
        }

        #endregion

        /************************ WINDOWS CHROME FUNCTIOANLITY (keeps taskbar visible) *********************/
        #region TaskbarVisiblity
        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>x coordinate of point.</summary>
            public int x;
            /// <summary>y coordinate of point.</summary>
            public int y;
            /// <summary>Construct a point of coordinates (x,y).</summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public static readonly RECT Empty = new RECT();
            public int Width { get { return Math.Abs(right - left); } }
            public int Height { get { return bottom - top; } }
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }
            public bool IsEmpty { get { return left >= right || top >= bottom; } }
            public override string ToString()
            {
                if (this == Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }
            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }
            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion
    }
}