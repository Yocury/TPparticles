﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp5
{
    public class Particle
    {
        public int Radius;
        public float X; 
        public float Y; 

        public float SpeedX; 
        public float SpeedY;
        public float Life;

        public bool tp = false;

        public static Random rand = new Random();

        public Color Color;
        public float Opacity = 0; // От 0 до 1
        public float LifeTime = 60; // Уменьшили время жизни с 100 до 60
        public bool IsAppearing = true; // Флаг, показывающий появляется ли частица или исчезает

        public virtual void Draw(Graphics g)
        {
            float k = Math.Min(1f, Life / 100);
            // рассчитываем значение альфа канала в шкале от 0 до 255
            // по аналогии с RGB, он используется для задания прозрачности
            int alpha = (int)(k * 255);

            // создаем цвет из уже существующего, но привязываем к нему еще и значение альфа канала
            var color = Color.FromArgb(alpha, Color.White);
            var b = new SolidBrush(color);

            // остальное все так же
            g.FillEllipse(b, X - Radius, Y - Radius, Radius * 2, Radius * 2);

            b.Dispose();
        }

        public class ParticleColorful : Particle
        {
            // два новых поля под цвет начальный и конечный
            public Color FromColor;
            public Color ToColor;

            // для смеси цветов
            public static Color MixColor(Color color1, Color color2, float k)
            {
                return Color.FromArgb(
                    (int)(color2.A * k + color1.A * (1 - k)),
                    (int)(color2.R * k + color1.R * (1 - k)),
                    (int)(color2.G * k + color1.G * (1 - k)),
                    (int)(color2.B * k + color1.B * (1 - k))
                );
            }

            // ну и отрисовку перепишем
            public override void Draw(Graphics g)
            {
                float k = Math.Min(1f, Life / 100);
                if (k < 0)
                {
                    k = 0;
                    
                }
                // так как k уменьшается от 1 до 0, то порядок цветов обратный
                var color = MixColor(ToColor, FromColor, k);
                var b = new SolidBrush(color);

                g.FillEllipse(b, X - Radius, Y - Radius, Radius * 2, Radius * 2);

                b.Dispose();
            }
        }

        public void Update()
        {
            if (IsAppearing)
            {
                Opacity += 0.1f;
                if (Opacity >= 1)
                {
                    Opacity = 1;
                    IsAppearing = false;
                }
            }
            else
            {
                LifeTime -= 1.0f; // Увеличили скорость уменьшения времени жизни (было 0.5f)
                if (LifeTime <= 15) // Уменьшили порог начала исчезновения (было 30)
                {
                    Opacity = LifeTime / 15; // Изменили делитель для более быстрого исчезновения (было 30)
                }
            }
        }

        public bool IsDead()
        {
            return LifeTime <= 0 || Opacity <= 0;
        }
    }
}
