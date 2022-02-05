using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace WpfHashlipsJSONConverter
{
    public partial class MainWindow
    {
        public async void ShowJsonTemplate()
        {
            MemoryStream stream;
            JsonSerializerOptions options;
            string json = string.Empty;
            string jsonDisplay = String.Empty;
            switch (SelectedCollection)
            {
                case "InImage":
                    try
                    {
                        InImage image = new();
                        options = new JsonSerializerOptions { WriteIndented = true };
                        using (stream = new MemoryStream())
                        {
                            await System.Text.Json.JsonSerializer.SerializeAsync(stream, image, options);
                            stream.Position = 0;
                            using (var reader = new StreamReader(stream))
                                json = await reader.ReadToEndAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;

                case "Eyeball9":
                    try
                    {
                        Eyeball9 eyeball = new();
                        options = new JsonSerializerOptions { WriteIndented = true };
                        using (stream = new MemoryStream())
                        {
                            await System.Text.Json.JsonSerializer.SerializeAsync(stream, eyeball, options);
                            stream.Position = 0;
                            using (var reader = new StreamReader(stream))
                                json = await reader.ReadToEndAsync();
                        }
                    }
                    catch (Exception mem)
                    {
                        MessageBox.Show(mem.ToString());
                    }
                    break;

                default:
                    break;
            }

            //templateDisplay.DataContext = this;

            templateDisplay.Text = json;
            templateDisplay.Visibility = System.Windows.Visibility.Visible;

            ////Debug.WriteLine(json);
            open.IsChecked = false;
        }
    }
}