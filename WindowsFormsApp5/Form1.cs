using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp5.IImpactPoint;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        private List<Particle> particles = new List<Particle>();
        private Random random = new Random();
        private const int MAX_GREEN_PARTICLES = 3;
        
        // Таймеры для задержки создания частиц
        private float redSpawnDelay = 0;
        private float greenSpawnDelay = 0;
        
        // Настройки сложности
        private float minSpawnDelay;
        private float maxSpawnDelay;
        private int particleRadius;
        private float redParticleProbability;
        private float particleLifeTime; // Добавляем время жизни как параметр сложности
        private MenuForm.Difficulty currentDifficulty; // Сохраняем текущую сложность
        
        // Система очков
        private float score = 0;
        private Label topRightScoreLabel; // Новая метка для счёта справа сверху
        private Label lastPointsLabel; // Метка для отображения последнего изменения очков
        private Timer pointsFadeTimer; // Таймер для исчезновения сообщения о последних очках

        public Form1()
        {
            InitializeComponent();

            // Настраиваем счет справа сверху
            topRightScoreLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(this.ClientSize.Width - 150, 20),
                BackColor = Color.FromArgb(200, 255, 255, 255),
                ForeColor = Color.DarkGreen,
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(topRightScoreLabel);

            // Обработчик изменения размера формы
            this.Resize += (s, e) =>
            {
                topRightScoreLabel.Location = new Point(this.ClientSize.Width - topRightScoreLabel.Width - 20, 20);
            };

            // Настраиваем метку для отображения изменения очков
            lastPointsLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Arial", 20, FontStyle.Bold),
                Location = new Point(20, 80),
                BackColor = Color.Transparent,
                ForeColor = Color.Green,
                Visible = false
            };
            this.Controls.Add(lastPointsLabel);

            // Настраиваем таймер для исчезновения сообщения
            pointsFadeTimer = new Timer();
            pointsFadeTimer.Interval = 1000; // 1 секунда
            pointsFadeTimer.Tick += (s, e) =>
            {
                lastPointsLabel.Visible = false;
                pointsFadeTimer.Stop();
            };

            // Показываем меню выбора сложности
            using (var menuForm = new MenuForm())
            {
                if (menuForm.ShowDialog() != DialogResult.OK)
                {
                    // Если пользователь закрыл окно без выбора, закрываем игру
                    Application.Exit();
                    return;
                }

                currentDifficulty = menuForm.SelectedDifficulty;
                // Устанавливаем параметры в зависимости от выбранной сложности
                SetDifficultyParameters(currentDifficulty);
            }

            // Создаем bitmap после инициализации формы
            if (picDisplay != null)
            {
                picDisplay.Image = new Bitmap(picDisplay.Width, picDisplay.Height);
                timer1.Interval = 16;
                timer1.Start();
            }

            UpdateScoreDisplay();
        }

        private void UpdateScoreDisplay()
        {
            string difficultyText = "";
            switch (currentDifficulty)
            {
                case MenuForm.Difficulty.Easy:
                    difficultyText = "Лёгкий";
                    break;
                case MenuForm.Difficulty.Medium:
                    difficultyText = "Средний";
                    break;
                case MenuForm.Difficulty.Hard:
                    difficultyText = "Сложный";
                    break;
                default:
                    difficultyText = "";
                    break;
            }
            
            if (topRightScoreLabel != null)
            {
                topRightScoreLabel.Text = $"Счёт: {score:F1}";
                topRightScoreLabel.Location = new Point(this.ClientSize.Width - topRightScoreLabel.Width - 20, 20);
                topRightScoreLabel.BringToFront();
            }
            
            this.Invalidate();
            this.Update();
        }

        private void ShowPointsChange(float points, bool isRed = false)
        {
            string prefix = points > 0 ? "+" : "";
            lastPointsLabel.Text = $"{prefix}{points:F1}";
            
            // Устанавливаем цвет в зависимости от типа очков
            if (isRed)
            {
                lastPointsLabel.ForeColor = Color.Red;
            }
            else
            {
                lastPointsLabel.ForeColor = points >= 1 ? Color.Green : Color.Orange;
            }

            // Показываем метку
            lastPointsLabel.Visible = true;
            
            // Перезапускаем таймер
            pointsFadeTimer.Stop();
            pointsFadeTimer.Start();
        }

        private void RestartGame()
        {
            particles.Clear();
            score = 0;
            UpdateScoreDisplay(); // Обновляем отображение счёта при перезапуске
            
            using (var menuForm = new MenuForm())
            {
                if (menuForm.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }

                currentDifficulty = menuForm.SelectedDifficulty;
                SetDifficultyParameters(currentDifficulty);
            }

            UpdateScoreDisplay(); // Обновляем отображение после выбора сложности
            
            timer1.Start();
        }

        private void GameOver()
        {
            timer1.Stop();
            
            // Создаем форму для отображения результата
            Form gameOverForm = new Form
            {
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Text = "Конец игры"
            };

            // Заголовок
            Label titleLabel = new Label
            {
                AutoSize = false,
                Size = new Size(360, 50),
                Location = new Point(20, 20),
                Font = new Font("Arial", 24, FontStyle.Bold),
                Text = "Игра окончена!",
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Текущий счёт
            Label scoreLabel = new Label
            {
                AutoSize = false,
                Size = new Size(360, 100),
                Location = new Point(20, 80),
                Font = new Font("Arial", 20, FontStyle.Bold),
                Text = $"Ваш счёт:\n{score:F1}",
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Кнопка "Играть ещё раз"
            Button restartButton = new Button
            {
                Text = "Играть ещё раз",
                Size = new Size(160, 40),
                Location = new Point(40, 200),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.LightGreen
            };
            restartButton.Click += (s, e) =>
            {
                gameOverForm.DialogResult = DialogResult.Retry;
                gameOverForm.Close();
            };

            // Кнопка "Завершить"
            Button exitButton = new Button
            {
                Text = "Завершить",
                Size = new Size(160, 40),
                Location = new Point(200, 200),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.LightCoral
            };
            exitButton.Click += (s, e) =>
            {
                gameOverForm.DialogResult = DialogResult.Cancel;
                gameOverForm.Close();
            };

            gameOverForm.Controls.AddRange(new Control[] { titleLabel, scoreLabel, restartButton, exitButton });

            // Обрабатываем результат
            if (gameOverForm.ShowDialog() == DialogResult.Retry)
            {
                RestartGame();
            }
            else
            {
                Application.Exit();
            }
        }

        private void SetDifficultyParameters(MenuForm.Difficulty difficulty)
        {
            switch (difficulty)
            {
                case MenuForm.Difficulty.Easy:
                    minSpawnDelay = 0.3f;
                    maxSpawnDelay = 0.6f;
                    particleRadius = 20;
                    redParticleProbability = 0.4f;
                    particleLifeTime = 60f;
                    break;
                case MenuForm.Difficulty.Medium:
                    minSpawnDelay = 0.15f;
                    maxSpawnDelay = 0.4f;
                    particleRadius = 15;
                    redParticleProbability = 0.6f;
                    particleLifeTime = 35f; // Уменьшили с 45f до 35f для более быстрого исчезновения
                    break;
                case MenuForm.Difficulty.Hard:
                    minSpawnDelay = 0.05f;
                    maxSpawnDelay = 0.2f;
                    particleRadius = 10;
                    redParticleProbability = 0.8f;
                    particleLifeTime = 40f; // Увеличили с 30f до 40f для небольшого увеличения времени жизни
                    break;
            }
        }

        private void CreateParticlesIfNeeded()
        {
            // Удаляем мертвые частицы
            particles.RemoveAll(p => p.IsDead());

            // Подсчитываем текущее количество частиц каждого цвета
            int greenCount = particles.Count(p => p.Color == Color.Green);
            int redCount = particles.Count(p => p.Color == Color.Red);

            // Обработка таймеров
            float deltaTime = timer1.Interval / 1000f; // Переводим миллисекунды в секунды

            // Обработка появления красной частицы
            if (redCount == 0)
            {
                if (redSpawnDelay <= 0)
                {
                    // Проверяем вероятность появления красной частицы
                    if (random.NextDouble() < redParticleProbability)
                    {
                        var particle = new Particle
                        {
                            X = random.Next(50, picDisplay.Width - 50),
                            Y = random.Next(50, picDisplay.Height - 50),
                            Radius = particleRadius,
                            Color = Color.Red,
                            Opacity = 0,
                            LifeTime = particleLifeTime,
                            IsAppearing = true
                        };
                        particles.Add(particle);
                    }
                    redSpawnDelay = (float)(random.NextDouble() * (maxSpawnDelay - minSpawnDelay) + minSpawnDelay);
                }
                else
                {
                    redSpawnDelay -= deltaTime;
                }
            }

            // Обработка появления зеленых частиц
            if (greenCount < MAX_GREEN_PARTICLES)
            {
                if (greenSpawnDelay <= 0)
                {
                    var particle = new Particle
                    {
                        X = random.Next(50, picDisplay.Width - 50),
                        Y = random.Next(50, picDisplay.Height - 50),
                        Radius = particleRadius,
                        Color = Color.Green,
                        Opacity = 0,
                        LifeTime = particleLifeTime,
                        IsAppearing = true
                    };
                    particles.Add(particle);
                    greenCount++;
                    greenSpawnDelay = (float)(random.NextDouble() * (maxSpawnDelay - minSpawnDelay) + minSpawnDelay);
                }
                else
                {
                    greenSpawnDelay -= deltaTime;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (picDisplay.Image == null) return;

            // Обновляем состояние частиц
            foreach (var particle in particles)
            {
                particle.Update();
            }

            CreateParticlesIfNeeded();

            using (var g = Graphics.FromImage(picDisplay.Image))
            {
                g.Clear(Color.White);
                foreach (var particle in particles)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(
                        (int)(particle.Opacity * 255),
                        particle.Color)))
                    {
                        g.FillEllipse(brush,
                            particle.X - particle.Radius,
                            particle.Y - particle.Radius,
                            particle.Radius * 2,
                            particle.Radius * 2);
                    }
                }
            }
            picDisplay.Invalidate();
        }

        private void picDisplay_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var particle = particles[i];
                if (Math.Pow(e.X - particle.X, 2) + Math.Pow(e.Y - particle.Y, 2) <= Math.Pow(particle.Radius, 2))
                {
                    if (particle.Color == Color.Green)
                    {
                        float points;
                        if (particle.LifeTime <= 15)
                        {
                            points = 0.5f;
                            score += points;
                        }
                        else
                        {
                            points = 1.0f;
                            score += points;
                        }
                        ShowPointsChange(points);
                        particles.RemoveAt(i);
                        greenSpawnDelay = (float)(random.NextDouble() * (maxSpawnDelay - minSpawnDelay) + minSpawnDelay);
                    }
                    else if (particle.Color == Color.Red)
                    {
                        switch (currentDifficulty)
                        {
                            case MenuForm.Difficulty.Easy:
                                score -= 2;
                                ShowPointsChange(-2, true);
                                break;
                            case MenuForm.Difficulty.Medium:
                                score -= 10;
                                ShowPointsChange(-10, true);
                                break;
                            case MenuForm.Difficulty.Hard:
                                GameOver();
                                return;
                        }
                        particles.RemoveAt(i);
                    }
                    UpdateScoreDisplay(); // Обновляем отображение счёта после любого изменения
                    break;
                }
            }
        }
    }
}
