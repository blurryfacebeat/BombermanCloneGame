using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boomberman
{
    public delegate void deClear();

    public partial class Form1 : Form
    {
        MainBoard board;

        int level = 1;

        public Form1()
        {
            InitializeComponent();
            ////Инициализируем нашу панель
            NewGame();
        }
        //Инициализируем нашу панель
        private void NewGame()
        {
            board = new MainBoard(GamePanel, StartClear, ScoreLabel);
            ChangeLevel(level);
            timerGameOver.Enabled = true;
        }

        private void обИгреToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"
Игра является проектом на курсовую работу.
Вы управляете персонажем в виде человека на прямоугольном уровне, содержащем неразрушаемые и разрушаемые кирпичичные блоки.
Вы можете класть бомбы, которые взрываются в течение нескольких секунд.
Взрывая кирпичные блоки, вы получаете бонусы, которые увеличивают определенную характеристику.
По уровню перемещаются враги. Вы умираете от соприкосновения с ними, либо от бомб
Ваша задача: убить всех противников на уровне.
                            ", "Описание игры");
        }

        private void обАвтореToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"
Автор Игры: Морозов Вячеслав. 
Студент ТУСУРА второго курса, направления Программная Инженерия.
Почтовый ящик: blurryfacebeat@gmail.com                           ", "Об авторе");
        }

        //Нажимаем на кнопку, двигаемся, все просто
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (timerGameOver.Enabled)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        board.PlayerMove(Arrows.left);
                        break;
                    case Keys.Right:
                        board.PlayerMove(Arrows.right);
                        break;
                    case Keys.Up:
                        board.PlayerMove(Arrows.up);
                        break;
                    case Keys.Down:
                        board.PlayerMove(Arrows.down);
                        break;
                    case Keys.Space:
                        board.PutBomb();
                        break;
                }
            }
        }

        //Таймер очистки панели от спрайтов огня
        private void timerFireClear_Tick(object sender, EventArgs e)
        {
            board.ClearFire();
            timerFireClear.Enabled = false;
        }

        //Запуск таймера очистки огня
        private void StartClear()
        {
            timerFireClear.Enabled = true;
        }

        //Таймер окончания игры
        private void timerGameOver_Tick(object sender, EventArgs e)
        {
            if (board.GameOver())
            {
                timerGameOver.Enabled = false;
                DialogResult dr = MessageBox.Show("Попробовать снова?", "Игра окончена!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    NewGame();
                }
            }
        }

        private void новаяИграToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void управлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"
Управление стрелками:
Влево, Вправо, Вверх, Вниз

Поставить бомбу:
Пробел", "Управление");
        }

        private void уровниСложностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"
Легкая:
Противник выбирает доступную точку и бежит к ней

Средняя:
Противник выбирает доступную точку и бежит к ней, но, если видит бомбу или огонь - убегает

Тяжелая:
Противник выбирает доступную точку и бежит к ней, но, если видит бомбу или огонь - убегает, если видит персонажа - бежит к нему", "Об авторе");
        }

        private void легкаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLevel(1);
        }

        private void средняяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLevel(2);
        }

        private void тяжелаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLevel(3);
        }

        private void ChangeLevel(int _level)
        {
            level = _level;
            board.SetEnemiesLevel(level);
        }

        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ScoreLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
