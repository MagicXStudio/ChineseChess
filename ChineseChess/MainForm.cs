using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace ChineseChess
{
    public partial class MainForm : Form
    {
        private GameController gameController { get; set; }

        private Dictionary<int, Image> PieceImagePair { get; set; }  //����int-Image�Ķ�Ӧ

        private const int RowSum = 10;  //������
        private const int ColSum = 9;   //������

        private int[] LastStep { get; set; } //��¼��һ���Ķ��������ڳ�������

        private ChessPiece Selected { get; set; }  //��ǰѡ�е�����

        private Image SelectedImage { get; set; }   //��ǰѡ�е����ӵ�ͼ����������Ч����ʵ��

        private System.Timers.Timer flickerTimer { get; set; }  //��ʱ������������Ч����ʵ��

        private int[][] moveHelper { get; set; } //���ڴ��浱ǰѡ�������ӿɽӴ����������ƶ�������λ��״̬


        public MainForm()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.skipToolStripMenuItem.Enabled = false;
            this.undoToolStripMenuItem.Enabled = false;
            gameController = new GameController();
            flickerTimer = new System.Timers.Timer();
        }

        //�Ҽ��˵�-����Ϸ
        private void NewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //����Ϸ�ĳ�ʼ������
            if (this.panelChessman.Controls.Count == 0)
            {
                //���û�����ɹ�����PictureBox����ô����10 * 9��
                for (int j = 0; j < RowSum; ++j)
                {
                    for (int i = 0; i < ColSum; ++i)
                    {
                        ChessPiece pictureBox = new ChessPiece();
                        pictureBox.Size = new Size(70, 70);
                        if (j < 5)
                        {
                            pictureBox.Location = new Point(i * 83, 10 + (j * 83));
                        }
                        else if (j >= 5)
                        {
                            pictureBox.Location = new Point(i * 83, 15 + (j * 83));
                        }
                        pictureBox.Parent = panelChessman;
                        pictureBox.BackColor = Color.Transparent;
                        pictureBox.Click += new EventHandler(ClickChessEvent);
                        pictureBox.MouseEnter += new EventHandler(MouseEnterEvent);
                        pictureBox.MouseLeave += new EventHandler(MouseExitEvent);
                        panelChessman.Controls.Add(pictureBox);
                    }
                }


                flickerTimer.Elapsed += new ElapsedEventHandler(TimerPictureBoxFlicker);
                flickerTimer.Interval = 500;
                flickerTimer.AutoReset = true;

                //��ӱ�ź�Image��Ӧ���ֵ�
                PieceImagePair = new Dictionary<int, Image>();
                PieceImagePair.Add(-7, Properties.Resources.enemy7); PieceImagePair.Add(7, Properties.Resources.friend7);
                PieceImagePair.Add(-6, Properties.Resources.enemy6); PieceImagePair.Add(6, Properties.Resources.friend6);
                PieceImagePair.Add(-5, Properties.Resources.enemy5); PieceImagePair.Add(5, Properties.Resources.friend5);
                PieceImagePair.Add(-4, Properties.Resources.enemy4); PieceImagePair.Add(4, Properties.Resources.friend4);
                PieceImagePair.Add(-3, Properties.Resources.enemy3); PieceImagePair.Add(3, Properties.Resources.friend3);
                PieceImagePair.Add(-2, Properties.Resources.enemy2); PieceImagePair.Add(2, Properties.Resources.friend2);
                PieceImagePair.Add(-1, Properties.Resources.enemy1); PieceImagePair.Add(1, Properties.Resources.friend1);
                PieceImagePair.Add(0, null);
                PieceImagePair.Add(10, Properties.Resources.green); PieceImagePair.Add(-10, Properties.Resources.red);

                //��̬��������ռ�
                LastStep = new int[6];
            }

            //������Ϸ�����°ڷ�����
            gameController.Reset();
            this.ResetAllChessman();
            //��ʼ��������ʱ��
            DisableFlickerTimer();
            //�����Ҽ��˵�
            this.skipToolStripMenuItem.Enabled = true;
            this.undoToolStripMenuItem.Enabled = true;
            //��ʼ������
            SetLastStep(-1, -1, -1, -1, -1, -1);
        }

        //������������
        private void ResetAllChessman()
        {
            int index = 0;
            foreach (Control control in this.panelChessman.Controls)
            {
                ChessPiece pictureBox = (ChessPiece)control;
                pictureBox.Image = PieceImagePair[gameController.GetChessman(index / ColSum, index % ColSum)];
                index++;
            }
        }

        //�ƶ���������
        private bool ResetAChessman(int lasti, int lastj, int i, int j)
        {
            //��Ϸ�Ƿ񼴽�����
            bool gameWillOver = false;
            if (true == gameController.CanMove(lasti, lastj, i, j) && true == gameController.IsGameOver(i, j))
                gameWillOver = true;
            //�ƶ�
            SetLastStep(lasti, lastj, i, j, gameController.GetChessman(lasti, lastj), gameController.GetChessman(i, j));
            gameController.SetChessman(lasti, lastj, i, j);
            PictureBox pictureBox = (PictureBox)this.panelChessman.Controls[i * ColSum + j];
            pictureBox.Image = PieceImagePair[gameController.GetChessman(i, j)];
            PictureBox oldPictureBox = (PictureBox)this.panelChessman.Controls[lasti * ColSum + lastj];
            oldPictureBox.Image = PieceImagePair[0];
            if (gameWillOver)
            {
                bool winner = gameController.GameOver();
                SelectedImage = null;
                DisableFlickerTimer();
                this.skipToolStripMenuItem.Enabled = false;
                this.undoToolStripMenuItem.Enabled = false;
                this.Cursor = Cursors.Default;
                MessageBox.Show("��Ϸ������" + (winner == false ? "��" : "��") + "��ʤ��");
            }
            return true;
        }

        //��������
        private void UpdateChessPanel()
        {
            for (int i = 0; i < RowSum; ++i)
                for (int j = 0; j < ColSum; ++j)
                    if (gameController.GetMoveHelper(i, j) == 1)
                        ((ChessPiece)this.panelChessman.Controls[i * ColSum + j]).BackgroundImage = PieceImagePair[-10];
                    else if (gameController.GetMoveHelper(i, j) == -1)
                        ((ChessPiece)this.panelChessman.Controls[i * ColSum + j]).BackgroundImage = PieceImagePair[0];
                    else if (gameController.GetMoveHelper(i, j) == 2)
                        ((PictureBox)this.panelChessman.Controls[i * ColSum + j]).BackgroundImage = PieceImagePair[10];
        }

        //��¼��һ������Ϣ
        private void SetLastStep(int lasti, int lastj, int i, int j, int lastValue, int value)
        {
            LastStep[0] = lasti;
            LastStep[1] = lastj;
            LastStep[2] = i;
            LastStep[3] = j;
            LastStep[4] = lastValue;
            LastStep[5] = value;
        }

        //�Ҽ��˵�-�����غ�
        private void SkipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveHelper();
            DisableFlickerTimer();
            gameController.SkipTurn();
        }

        //�Ҽ��˵�-����
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LastStep[0] == -1)
                return;
            RemoveHelper();
            DisableFlickerTimer();
            gameController.ResetChessman(LastStep[0], LastStep[1], LastStep[2], LastStep[3], LastStep[4], LastStep[5]);
            PictureBox pictureBox = (PictureBox)this.panelChessman.Controls[LastStep[2] * ColSum + LastStep[3]];
            pictureBox.Image = PieceImagePair[LastStep[5]];
            PictureBox oldPictureBox = (PictureBox)this.panelChessman.Controls[LastStep[0] * ColSum + LastStep[1]];
            oldPictureBox.Image = PieceImagePair[LastStep[4]];
            SetLastStep(-1, -1, -1, -1, -1, -1);
        }

        //�Ҽ��˵�-�˳�
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //������������¼�
        private void MouseEnterEvent(object sender, EventArgs e)
        {
            //��ȡ��ǰ��PictureBox
            PictureBox pictureBox = (PictureBox)sender;
            //��ǰû��ѡ���Ҹõ�Ϊ��
            if (Selected == null && pictureBox.Image == null)
            {
                this.Cursor = Cursors.No;
            }
            //��ǰû��ѡ��
            else if (Selected == null)
            {
                int index = 0;
                foreach (Control control in this.panelChessman.Controls)
                {
                    if (pictureBox == (PictureBox)control)
                        break;
                    index++;
                }
                if (gameController.IsAvaliable(index / ColSum, index % ColSum) == false)
                {
                    this.Cursor = Cursors.No;
                }
                else
                {
                    this.Cursor = Cursors.Hand;
                }
            }
            //��ǰ��ѡ�е�
            else
            {
                //�����ѡ�У����ж��ǲ���ͬһ�ߵģ�����ǣ�ָ��ĳ�Hand
                //�����ѡ�У���ô�����ƶ����Ȼ�ȡ��ЩPictureBox��λ��
                PictureBox lastPictureBox = Selected;
                int index = 0, thisIndex = 0, lastIndex = 0;
                foreach (Control control in this.panelChessman.Controls)
                {
                    if (lastPictureBox == (PictureBox)control)
                        lastIndex = index;
                    if (pictureBox == (PictureBox)control)
                        thisIndex = index;
                    index++;
                }
                //�����ƶ����鿴��������ǲ�ʵ���ƶ�
                /*if ((gameController.GetChessman(lastIndex / ColSum, lastIndex % ColSum) < 0 && gameController.GetChessman(thisIndex / ColSum, thisIndex % ColSum) < 0)
                 || (gameController.GetChessman(lastIndex / ColSum, lastIndex % ColSum) > 0 && gameController.GetChessman(thisIndex / ColSum, thisIndex % ColSum) > 0)
                 || true == gameController.CanMove(lastIndex / ColSum, lastIndex % ColSum, thisIndex / ColSum, thisIndex % ColSum))*/
                if (gameController.GetMoveHelper(thisIndex / ColSum, thisIndex % ColSum) != 0)
                    this.Cursor = Cursors.Hand;
                else
                    this.Cursor = Cursors.No;
            }
        }

        //����Ƴ������¼�
        private void MouseExitEvent(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        //����������¼�
        private void ClickChessEvent(object sender, EventArgs e)
        {
            //��ȡ��ǰ��PictureBox
            ChessPiece pictureBox = (ChessPiece)sender;
            //�ж��Ƿ��Ѿ���ѡ��
            if (Selected == null && pictureBox.Image == null)
                return;
            if (Selected == null)
            {
                int index = 0;
                foreach (Control control in this.panelChessman.Controls)
                {
                    if (pictureBox == (PictureBox)control)
                        break;
                    index++;
                }
                if (gameController.IsAvaliable(index / ColSum, index % ColSum))
                {
                    SetHelper(index / ColSum, index % ColSum);
                    Selected = pictureBox;
                    flickerTimer.Enabled = true;
                }
            }
            else if (Selected == pictureBox)
            {
                RemoveHelper();
                DisableFlickerTimer();
            }
            else
            {
                //�����ѡ�У���ô�����ƶ����Ȼ�ȡ��ЩPictureBox��λ��
                PictureBox lastPictureBox = Selected;
                int index = 0, thisIndex = 0, lastIndex = 0;
                foreach (Control control in this.panelChessman.Controls)
                {
                    if (lastPictureBox == (PictureBox)control)
                        lastIndex = index;
                    if (pictureBox == (PictureBox)control)
                        thisIndex = index;
                    index++;
                }
                //�����ƶ�����������ƶ�����ô�ȸı��ٽ��ã���������ƶ�����ô�Ƚ����ٸı�
                //���ƶ�֮ǰ�Ƴ���ʾ�򣬷�ֹ������
                RemoveHelper();
                if (true == gameController.CanMove(lastIndex / ColSum, lastIndex % ColSum, thisIndex / ColSum, thisIndex % ColSum))
                {
                    this.ResetAChessman(lastIndex / ColSum, lastIndex % ColSum, thisIndex / ColSum, thisIndex % ColSum);
                    this.Cursor = Cursors.No;
                    Selected = pictureBox;
                    DisableFlickerTimer();
                }
                else
                {
                    DisableFlickerTimer();
                    Selected = null;
                }
            }
        }

        //������ʾ��
        private void SetHelper(int i, int j)
        {
            gameController.SetMoveHelper(i, j);
            UpdateChessPanel();
        }

        //�Ƴ���ʾ��
        private void RemoveHelper()
        {
            gameController.SetMoveHelper(-1, -1);
            UpdateChessPanel();
        }

        //����������ʱ����û��ѡȡ�����ӻ���ѡȡ�������Ѿ�����˱����ƶ�
        private void DisableFlickerTimer()
        {
            if (SelectedImage != null)
                Selected.Image = SelectedImage;
            flickerTimer.Enabled = false;
            SelectedImage = null;
            Selected = null;
        }

        //�ؼ������¼�����Image��NULL֮�佻���Դﵽ����Ч��
        private void TimerPictureBoxFlicker(object sender, EventArgs e)
        {
            if (Selected != null && SelectedImage == null)
                SelectedImage = Selected.Image;
            if (Selected.Image == null)
                Selected.Image = SelectedImage;
            else
                Selected.Image = null;
        }

        //����ܼ��غ���
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.panelChessman.Parent = this.pictureBoxChessPanel;
        }
    }
}
