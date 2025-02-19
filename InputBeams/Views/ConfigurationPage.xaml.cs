using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace InputBeams.Views
{
    public sealed partial class ConfigurationPage : Page
    {
        public ConfigurationPage()
        {
            InitializeComponent();
        }

        private void OnSensitivityValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (SensitivityValueText != null)
            {
                SensitivityValueText.Text = e.NewValue.ToString("F0"); // Display as integer
            }
        }
    }
}
