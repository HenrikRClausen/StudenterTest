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

        private bool RetStudent(Student AtRette) {
            if (AtRette == null)
                return false;
            StudentForm SF = new StudentForm();
            SF.Title = "Ret studenterdata";
            SF.firstName.Text = AtRette.FirstName;
            SF.lastName.Text = AtRette.LastName;
            SF.dateOfBirth.Text = AtRette.DateOfBirth.ToString("d");
            if (SF.ShowDialog().Value) {
                AtRette.FirstName = SF.firstName.Text;
                AtRette.LastName = SF.lastName.Text;
                AtRette.DateOfBirth = DateTime.Parse(SF.dateOfBirth.Text);
                return true;
            }
            return false;
        }

        private bool NyStudent() {
            StudentForm Tilfoej = new StudentForm();
            Tilfoej.Title = "Tilføj ny student til " + teacher.Class;
            if (Tilfoej.ShowDialog().Value) {
                Student NyStudent = new Student();
                NyStudent.FirstName = Tilfoej.firstName.Text;
                NyStudent.LastName = Tilfoej.lastName.Text;
                NyStudent.DateOfBirth = DateTime.Parse(Tilfoej.dateOfBirth.Text);
                teacher.Students.Add(NyStudent);
                return true;
            }
            return false;
        }

        private bool SletStudent (Student SletMig) {
            if (SletMig == null)
                return false;
            var Svar = MessageBox.Show("Vil du slette " + SletMig.FirstName + " " + SletMig.LastName,
                                        "Slet studerende..?",
                                        MessageBoxButton.YesNo);
            if (Svar == MessageBoxResult.Yes) {
                schoolContext.DeleteObject(SletMig);
                return true;
                }
            return false;
        }

        // When the user presses a key, determine whether to add a new student to a class, remove a student from a class, or modify the details of a student
        private void studentsList_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Enter:
                    Student DenneStudent = studentsList.SelectedItem as Student;
                    if (RetStudent(DenneStudent))
                        saveChanges.IsEnabled = true;
                    break;
                case Key.Insert:
                    if (NyStudent())
                        saveChanges.IsEnabled = true;
                    break;
                case Key.Delete:
                    if (SletStudent(studentsList.SelectedItem as Student))
                        saveChanges.IsEnabled = true;
                    break;
                default:
                    break;
            }
        }

        #region Predefined code

        private void studentsList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Student DenneStudent = studentsList.SelectedItem as Student;
            if (RetStudent(DenneStudent))
                saveChanges.IsEnabled = true;
        }

        // Save changes back to the database and make them permanent
        private void saveChanges_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        private void StudentsList_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }
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

        #region Oprindelig kode 

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
