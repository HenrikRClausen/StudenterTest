using System;
using System.Windows;

namespace School
{
    /// <summary>
    /// Interaction logic for StudentForm.xaml
    /// </summary>
    public partial class StudentForm : Window
    {
        
        public StudentForm()
        {
            InitializeComponent();
        }

        #region Valideringskode 
        private void ok_Click(object sender, RoutedEventArgs e) {
            DateTime Dato;

            if (this.firstName.Text == "") {
                MessageBox.Show("Du skal indstaste et fornavn", "Fornavn mangler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.lastName.Text == "") {
                MessageBox.Show("Tak for fornavnet. Du skal også indstaste et efternavn", "Efternavn mangler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try {
                Dato = DateTime.Parse(this.dateOfBirth.Text);
            }
            catch {
                MessageBox.Show("Du skal også indtaste fødselsdato i formatet DD-MM-ÅÅÅÅ", "Ugyldig dato", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            TimeSpan Forskel = DateTime.Now.Subtract(Dato);
            int Alder = (int)(Forskel.Days / 365.25);

            if (Alder < 5) {
                MessageBox.Show("Skolen optager ikke elever yngre end fem år.", "Alt for ung!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.DialogResult = true;
        }
        #endregion
    }
}
