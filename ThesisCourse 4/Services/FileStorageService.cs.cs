using System.Diagnostics;
using System.IO;
using System.Text.Json;
using ThesisCourse_4.MVVM.Models;

namespace ThesisCourse_4.Services
{
    public interface IStorageService
    {
        IReadOnlyList<ButtonModel> LoadButtons();
        void SaveButtons(IEnumerable<ButtonModel> buttons);
        FullStorageModel? LoadAll();
        void SaveAll(IEnumerable<ButtonModel> buttons, IEnumerable<Node> nodes, IEnumerable<Edge> edges);
    }

    public class FileStorageService : IStorageService
    {
        private readonly string _buttonsFilePath;
        private readonly string _allDataFilePath;

        public FileStorageService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "ThesisCourse_4");
            Directory.CreateDirectory(folder);

            _buttonsFilePath = Path.Combine(folder, "buttons.txt");
            _allDataFilePath = Path.Combine(folder, "all_data.json");
        }

        public IReadOnlyList<ButtonModel> LoadButtons()
        {
            if (!File.Exists(_buttonsFilePath))
                return Array.Empty<ButtonModel>();

            try
            {
                var lines = File.ReadAllLines(_buttonsFilePath, System.Text.Encoding.UTF8);
                return lines
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(ButtonModel.FromString)
                    .ToList()
                    .AsReadOnly();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileStorage] Ошибка загрузки кнопок: {ex.Message}");
                return Array.Empty<ButtonModel>();
            }
        }

        public void SaveButtons(IEnumerable<ButtonModel> buttons)
        {
            try
            {
                var lines = buttons.Select(btn => btn.ToString());
                File.WriteAllLines(_buttonsFilePath, lines, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileStorage] Ошибка сохранения кнопок: {ex.Message}");
            }
        }

        public void SaveAll(IEnumerable<ButtonModel> buttons, IEnumerable<Node> nodes, IEnumerable<Edge> edges)
        {
            try
            {
                var model = new FullStorageModel
                {
                    Buttons = buttons.ToList(),
                    Nodes = nodes.ToList(),
                    Edges = edges.ToList()
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(model, options);
                File.WriteAllText(_allDataFilePath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileStorage] Ошибка сохранения всего: {ex.Message}");
            }
        }

        public FullStorageModel? LoadAll()
        {
            if (!File.Exists(_allDataFilePath))
                return null;

            try
            {
                var json = File.ReadAllText(_allDataFilePath);
                return JsonSerializer.Deserialize<FullStorageModel>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FileStorage] Ошибка загрузки всего: {ex.Message}");
                return null;
            }
        }
    }

    public class FullStorageModel
    {
        public List<ButtonModel> Buttons { get; set; } = new List<ButtonModel>();
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Edge> Edges { get; set; } = new List<Edge>();
    }
}
