using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class MenuForm : Form
    {
        public enum Difficulty { Easy, Medium, Hard }
        public Difficulty SelectedDifficulty { get; private set; }

        public MenuForm()
        {
            InitializeComponent();
            this.Text = "Выбор сложности";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Создаем заголовок
            Label titleLabel = new Label
            {
                Text = "Выберите уровень сложности",
                Font = new Font("Arial", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50
            };

            // Создаем кнопки
            Button easyButton = new Button
            {
                Text = "Легко",
                Size = new Size(200, 40),
                Location = new Point(100, 80),
                Font = new Font("Arial", 12),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            easyButton.Click += (s, e) => 
            {
                SelectedDifficulty = Difficulty.Easy;
                DialogResult = DialogResult.OK;
                Close();
            };

            Button mediumButton = new Button
            {
                Text = "Средне",
                Size = new Size(200, 40),
                Location = new Point(100, 140),
                Font = new Font("Arial", 12),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            mediumButton.Click += (s, e) =>
            {
                SelectedDifficulty = Difficulty.Medium;
                DialogResult = DialogResult.OK;
                Close();
            };

            Button hardButton = new Button
            {
                Text = "Сложно",
                Size = new Size(200, 40),
                Location = new Point(100, 200),
                Font = new Font("Arial", 12),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            hardButton.Click += (s, e) =>
            {
                SelectedDifficulty = Difficulty.Hard;
                DialogResult = DialogResult.OK;
                Close();
            };

            // Добавляем элементы на форму
            Controls.Add(titleLabel);
            Controls.Add(easyButton);
            Controls.Add(mediumButton);
            Controls.Add(hardButton);
        }
    }
} 