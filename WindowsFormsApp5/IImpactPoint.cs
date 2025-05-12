using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp5
{
    public abstract class IImpactPoint
    {
        public float X; // ну точка же, вот и две координатыs
        public float Y;

        // абстрактный метод с помощью которого будем изменять состояние частиц
        // например притягивать
        public abstract void ImpactParticle(Particle particle);

        // базовый класс для отрисовки точечки
        public virtual void Render(Graphics g)
        {
            g.FillEllipse(
                    new SolidBrush(Color.Red),
                    X - 5,
                    Y - 5,
                    10,
                    10
                );
        }

        public class InPoint : IImpactPoint
        {
            public int rad = 100;

            // а сюда по сути скопировали с минимальными правками то что было в UpdateState
            public override void ImpactParticle(Particle particle)
            {
                float gX = X - particle.X;
                float gY = Y - particle.Y;

                double r = Math.Sqrt(gX * gX + gY * gY); // считаем расстояние от центра точки до центра частицы
                if (r - particle.Radius < rad / 2) // если частица оказалось внутри окружности
                {
                    particle.tp = true;
                }
            }
            public override void Render(Graphics g)
            {
                // буду рисовать окружность с диаметром равным Power
                g.DrawEllipse(
                       new Pen(Color.Orange),
                       X - rad / 2,
                       Y - rad / 2,
                       rad,
                       rad
                   );
            }
        }

        public class OutPoint : IImpactPoint
        {
            public int rad = 30; 

            // а сюда по сути скопировали с минимальными правками то что было в UpdateState
            public override void ImpactParticle(Particle particle)
            {
                if (particle.tp)
                {
                    particle.tp = false;
                    particle.SpeedX *= -1;
                    particle.SpeedY *= -1;
                    particle.X = X;
                    particle.Y = Y;
                }
            }
            public override void Render(Graphics g)
            {
                // буду рисовать окружность с диаметром равным Power
                g.DrawEllipse(
                       new Pen(Color.Aqua),
                       X - rad / 2,
                       Y - rad / 2,
                       rad,
                       rad
                   );
            }
        }

        public class BouncePoint : IImpactPoint
        {
            public int rad = 100; 

            // а сюда по сути скопировали с минимальными правками то что было в UpdateState
            public override void ImpactParticle(Particle particle)
            {
                float gX = X - particle.X;
                float gY = Y - particle.Y;

                double r = Math.Sqrt(gX * gX + gY * gY); // считаем расстояние от центра точки до центра частицы
                if (r - particle.Radius < rad / 2) // если частица оказалось внутри окружности
                {
                    float nx = (float)(gX / r);
                    float ny = (float)(gY / r);

                    // Найти скалярное произведение скорости частицы и нормали
                    float dotProduct = particle.SpeedX * nx + particle.SpeedY * ny;

                    // Отразить скорость частицы относительно нормали
                    particle.SpeedX = particle.SpeedX - 2 * dotProduct * nx;
                    particle.SpeedY = particle.SpeedY - 2 * dotProduct * ny;
                }
            }
            public override void Render(Graphics g)
            {
                // буду рисовать окружность с диаметром равным Power
                g.DrawEllipse(
                       new Pen(Color.Red),
                       X - rad / 2,
                       Y - rad / 2,
                       rad,
                       rad
                   );
            }
        }
    }
}
