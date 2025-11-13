using System.Diagnostics;
using System.IO;
using System.Text;
using ThesisCourse_4.MVVM.Models;

namespace ThesisCourse_4.Services
{
    public interface IStorageService
    {
        IReadOnlyList<ButtonModel> LoadButtons();
        void SaveButtons(IEnumerable<ButtonModel> buttons);
    }

    public class FileStorageService : IStorageService
    {
        private readonly string _filePath;

        public FileStorageService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "ThesisCourse_4");
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, "buttons.txt");
        }

        public IReadOnlyList<ButtonModel> LoadButtons()
        {
            if (!File.Exists(_filePath))
                return Array.Empty<ButtonModel>();

            try
            {
                var lines = File.ReadAllLines(_filePath, Encoding.UTF8);
                return lines
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(ButtonModel.FromString)
                    .ToList()
                    .AsReadOnly();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileStorage] Ошибка загрузки: {ex.Message}");
                return Array.Empty<ButtonModel>();
            }
        }

        public void SaveButtons(IEnumerable<ButtonModel> buttons)
        {
            try
            {
                var lines = buttons.Select(btn => btn.ToString());
                File.WriteAllLines(_filePath, lines, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileStorage] Ошибка сохранения: {ex.Message}");
            }
        }
    }
}
