using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using School.Data;


namespace School {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        // Connection to the School database
        private SchoolDBEntities schoolContext = null;

        // Field for tracking the currently selected teacher
        private Teacher teacher = null;

        // List for tracking the students assigned to the teacher's class
        private IList studentsInfo = null;

        #region Veletableret kode

        public MainWindow() {
            InitializeComponent();
        }

        // Connect to the database and display the list of teachers when the window appears
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.schoolContext = new SchoolDBEntities();
            teachersList.DataContext = this.schoolContext.Teachers;
        }

        // When the user selects a different teacher, fetch and display the students for that teacher
        private void teachersList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // Find the teacher that has been selected
            this.teacher = teachersList.SelectedItem as Teacher;
            this.schoolContext.LoadProperty<Teacher>(this.teacher, s => s.Students);

            // Find the students for this teacher
            this.studentsInfo = ((IListSource)teacher.Students).GetList();

            // Use databinding to display these students
            studentsList.DataContext = this.studentsInfo;
        }

        #endregion

        // When the user presses a key, determine whether to add a new student to a class, remove a student from a class, or modify the details of a student
        private void studentsList_KeyDown(object sender, KeyEventArgs e) {
            // TODO: Exercise 1: Task 1a: If the user pressed Enter, edit the details for the currently selected student
            switch (e.Key) {
                case Key.Enter:
                    Student DenneStudent = studentsList.SelectedItem as Student;
                    if (DenneStudent == null)
                        break;      // Tomme data? Så kan vi ikke foretage os noget. Det burde ikke være muligt.
                    // TODO: Exercise 1: Task 2a: Use the StudentsForm to display and edit the details of the student
                    StudentForm SF = new StudentForm();
                    // TODO: Exercise 1: Task 2b: Set the title of the form and populate the fields on the form with the details of the student
                    SF.Title = "Ret studenterdata";
                    SF.firstName.Text = DenneStudent.FirstName;
                    SF.lastName.Text = DenneStudent.LastName;
                    SF.dateOfBirth.Text = DenneStudent.DateOfBirth.ToString("d");
                    // TODO: Exercise 1: Task 3a: Display the form
                    if (SF.ShowDialog().Value) {
                        // TODO: Exercise 1: Task 3b: When the user closes the form, copy the details back to the student
                        DenneStudent.FirstName = SF.firstName.Text;
                        DenneStudent.LastName = SF.lastName.Text;
                        DenneStudent.DateOfBirth = DateTime.Parse(SF.dateOfBirth.Text);
                        // TODO: Exercise 1: Task 3c: Enable saving (changes are not made permanent until they are written back to the database)
                        saveChanges.IsEnabled = true;
                    }
                    break;
                case Key.Insert:
                    StudentForm Tilfoej = new StudentForm();
                    Tilfoej.Title = "Tilføj ny student til " + teacher.Class;

                    if (Tilfoej.ShowDialog().Value) {
                        Student NyStudent = new Student();
                        NyStudent.FirstName = Tilfoej.firstName.Text;
                        NyStudent.LastName = Tilfoej.lastName.Text;
                        NyStudent.DateOfBirth = DateTime.Parse(Tilfoej.dateOfBirth.Text);
                        teacher.Students.Add(NyStudent);

                        saveChanges.IsEnabled = true;
                    }
                    break;
                case Key.Delete:
                    Student SletteStudent = studentsList.SelectedItem as Student;
                    if (SletteStudent == null)
                        break;
                    var Svar = MessageBox.Show("Vil du slette " + SletteStudent.FirstName + " " + SletteStudent.LastName, 
                                                "Slet studerende..?",
                                                MessageBoxButton.YesNo);
                    if (Svar == MessageBoxResult.Yes) {
                        schoolContext.DeleteObject(SletteStudent);
                        saveChanges.IsEnabled = true;
                    }
                    break;
                default:
                    break;
            }
        }

        #region Predefined code

        private void studentsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

        }

        // Save changes back to the database and make them permanent
        private void saveChanges_Click(object sender, RoutedEventArgs e) {

        }

        #endregion
    }

    [ValueConversion(typeof(string), typeof(Decimal))]
    class AgeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture) {
            if (value == null)
                return "";

            DateTime Dato = (DateTime)value;
            TimeSpan Forskel = DateTime.Now.Subtract(Dato);
            int Alder = (int)(Forskel.Days / 365.25);
            return Alder.ToString();
        }

        #region Predefined code

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
