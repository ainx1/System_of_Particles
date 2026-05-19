using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static particles.Emitter;
using static particles.Particle;

namespace particles
{
    public partial class Form1 : Form
    {
        List<Emitter> emitters = new List<Emitter>();
        Emitter emitter; // добавим поле для эмиттера

        GravityPoint point1; // добавил поле под первую точку
        GravityPoint point2; // добавил поле под вторую точку
                             
        ColorPoint colorPoint; // поле для раскрашивателя задача 5
        public Form1()
        {

            InitializeComponent();
            picDisplay.Image = new Bitmap(picDisplay.Width, picDisplay.Height);

            this.emitter = new Emitter // создаю эмиттер и привязываю его к полю emitter
            {
                Direction = 0,
                Spreading = 10,
                SpeedMin = 5,
                SpeedMax = 10,
                ColorFrom = Color.Gold,
                ColorTo = Color.FromArgb(0, Color.Black),
                ParticlesPerTick = 10,
                X = picDisplay.Width / 2,
                Y = picDisplay.Height / 2,
            };

            emitters.Add(this.emitter); // все равно добавляю в список emitters, чтобы он рендерился и обновлялся

            // привязываем гравитоны к полям
            point1 = new GravityPoint
            {
                X = picDisplay.Width / 2 + 100,
                Y = picDisplay.Height / 2,
            };
            point2 = new GravityPoint
            {
                X = picDisplay.Width / 2 - 100,
                Y = picDisplay.Height / 2,
            };

            // привязываем поля к эмиттеру
            emitter.impactPoints.Add(point1);
            emitter.impactPoints.Add(point2);

            // задача 5 раскрашиватель
            this.colorPoint = new ColorPoint
            {
                X = picDisplay.Width / 4, // Слева
                Y = picDisplay.Height / 1.5f,
                Color = Color.White 
            };
            emitter.impactPoints.Add(colorPoint);


            // задача 6 
             var counterPoint = new CounterPoint
            {
                X = picDisplay.Width / 2, // по центру
                Y = picDisplay.Height - 100,
                Power = 100
       
            };
            emitter.impactPoints.Add(counterPoint);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            emitter.UpdateState(); // каждый тик обновляем систему

            using (var g = Graphics.FromImage(picDisplay.Image))
            {
                g.Clear(Color.Black);
                emitter.Render(g); // рендерим систему
            }
            picDisplay.Invalidate();
        }
        private void picDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            // а тут передаем положение мыши, в положение гравитона
            point2.X = e.X;
            point2.Y = e.Y;
        }

        private void tbDirection_Scroll(object sender, EventArgs e)
        {
            emitter.Direction = tbDirection.Value; // направлению эмиттера присваиваем значение ползунка 
            lblDirection.Text = $"{tbDirection.Value}°"; // добавил вывод значения
        }

        private void tbGraviton1_Scroll(object sender, EventArgs e)
        {
            point1.Power = tbGraviton1.Value;
        }
            
        private void tbGraviton2_Scroll(object sender, EventArgs e)
        {
            point2.Power = tbGraviton2.Value;
        }

        private void tbColorPointPos_Scroll(object sender, EventArgs e)
        {
            // двигаем по оси X задача 5
            colorPoint.X = tbColorPointPos.Value;
        }

        // Задача 6
        private void picDisplay_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // создаем новый экземпляр счетчика кружка
                var newCounter = new CounterPoint
                {
                    X = e.X, // координата X клика
                    Y = e.Y, // координата Y клика
                    Power = 50 // радиус поглощения
                };

                emitter.impactPoints.Add(newCounter);
            }
        }
    }
}