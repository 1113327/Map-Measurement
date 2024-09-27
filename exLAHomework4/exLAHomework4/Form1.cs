using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;//Matrix matrix

namespace exLAHomework4
{
    public partial class Form1 : Form
    {
        Bitmap _bmpBG = new Bitmap(800, 600);
        Bitmap _bmp = new Bitmap(800, 600);        
        Point _ptS = new Point();
        Point _ptE = new Point();
        bool _draw = false;
        ArrayList _ptList = new ArrayList();

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Add("Rectangle");
            comboBox1.Items.Add("Pentagon");
            comboBox1.Items.Add("Ellipse");
            comboBox1.Items.Add("Polygon");
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _bmp = (Bitmap) _bmpBG.Clone();
            _ptS.X = e.X;
            _ptS.Y = e.Y;
            _draw = true;
            _ptList.Add(e.Location);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _ptE.X = e.X;
            _ptE.Y = e.Y;
            _draw = false;
            if (this.comboBox1.Text == "Line") CalculatelLine(_ptS, _ptE);
            else if (this.comboBox1.Text == "Triangle")
            {
                DrawTriangle(_ptList);
                CalculateTriangle();
            }
            else if (this.comboBox1.Text == "Rectangle")
            {
                DrawRectangle(_ptS, _ptE);
                CalculateRectangle(_ptS, _ptE);
            }
            else if (this.comboBox1.Text == "Pentagon")
            {
                DrawPentagon(_ptList);
                CalculatePentagon();
            }
            else if(this.comboBox1.Text == "Ellipse")
            {
                DrawEllipse(_ptS, _ptE);
                CalculateEllipse(_ptS, _ptE);
            }
            else if (this.comboBox1.Text == "Polygon")
            {
                // 只在滑鼠移動時添加新的點
                if (_draw)
                {
                    _ptList.Add(e.Location);
                }

                if (_ptList.Count >= 3)
                {
                    // 連接最後一點和起始點
                    DrawPolygon(_ptList, (Point)_ptList[0]);
                    CalculatePolygon();
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.comboBox1.Text == "Line")
            {
                if (!_draw) return;
                DrawLine(_ptS, new Point(e.X, e.Y));
            }
            else if (_draw && this.comboBox1.Text == "Polygon")
            {
                // 每次滑鼠移動時即時繪製多邊形的邊
                DrawPolygon(_ptList, e.Location);
            }
        }

        //Line
        private void CalculatelLine(Point ptS, Point ptE)
        {
            double X2 = Math.Pow(ptS.X - ptE.X, 2);
            double Y2 = Math.Pow(ptS.Y - ptE.Y, 2);
            double dist = Math.Sqrt(X2 + Y2);
            string strLog1 = string.Format("From ({0},{1}) to ({2},{3}) dist={4} \r\n", ptS.X, ptS.Y, ptE.X, ptE.Y, dist);
            string strLog2 = string.Format("Area = 0 \r\n\r\n");
            this.textBox1.Text += strLog1+strLog2 ;
        }
        private void DrawLine(Point ptS,Point ptE)
        {
            _bmp = (Bitmap) _bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);
            //g.Clear(Color.White);
            Pen pen = new Pen(Color.Purple, 3);
            g.DrawLine(pen, ptS, ptE);
            this.pictureBox1.Image = _bmp;
        }

        //Triangle
        private void CalculateTriangle()
        {
            if (_ptList.Count < 3) return;

            double distTotal = 0;
            double area = 0;

            StringBuilder coordinates = new StringBuilder("Coordinate: ");

            for (int idxS = 0; idxS < 3; idxS++)
            {
                int i = idxS;
                int idxE = (idxS + 1) % 3;

                Point ptS = (Point)_ptList[i];
                Point ptE = (Point)_ptList[idxE];

                double X2 = Math.Pow(((Point)_ptList[idxS]).X - ((Point)_ptList[idxE]).X, 2);
                double Y2 = Math.Pow(((Point)_ptList[idxS]).Y - ((Point)_ptList[idxE]).Y, 2);
                double dist = Math.Sqrt(X2 + Y2);

                distTotal += dist;
                area += (ptS.X * ptE.Y - ptE.X * ptS.Y);

                coordinates.AppendFormat("({0},{1})", ptS.X, ptS.Y);

                if (i < 2)
                {
                    coordinates.Append(" - ");
                }
            }

            distTotal = Math.Round(distTotal, 2);
            area = Math.Abs(area) / 2.0;

            string strLog1 = string.Format("total dist={0} \r\n", distTotal);
            string strLog2 = string.Format("Area = {0} \r\n\r\n", area);

            this.textBox1.Text += coordinates.ToString() + "\r\n" + strLog1 + strLog2;

            DrawTriangle(_ptList);
            _ptList.Clear();
        }

        private void DrawTriangle(ArrayList list)
        {
            _bmp = (Bitmap)_bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);            
            Pen pen = new Pen(Color.Purple, 3);
            for(int i=0;i<list.Count;i++)
            {
                Point pt1 = (Point)list[i];
                g.DrawEllipse(pen,pt1.X,pt1.Y  ,3,3);

                //if (list.Count<=2) continue;
                int idxE = (i + 1) % 3;
                if (idxE >= list.Count) continue;
                Point pt2 = (Point)list[idxE];
                g.DrawLine(pen, pt1,pt2);
            }            
            this.pictureBox1.Image = _bmp;
        }

        //Rectangle
        private void CalculateRectangle(Point ptS, Point ptE)
        {
            int width = Math.Abs(ptE.X - ptS.X);
            int height = Math.Abs(ptE.Y - ptS.Y);
            int distTotal = width + height;
            int area = width * height;
            /*沒旋轉的
            string strLog1 = string.Format("Rectangle: Width = {0}, Height = {1}, Area = {2} \r\n", width, height, area);
            string strLog2 = "Additional information about rectangle, if needed... \r\n\r\n";
            this.textBox1.Text += strLog1 + strLog2;
            // 繪製矩形邊界
            DrawRectangle(ptS, ptE);*/

            // 計算矩形四個頂點座標
            string strLog1 = string.Format("Coordinate: ({0},{1}) -\r", ptS.X, ptS.Y);
            string strLog2 = string.Format("({0},{1}) -\r", ptS.X + width, ptS.Y);
            string strLog3 = string.Format("({0},{1}) -\r", ptS.X + width, ptS.Y + height);
            string strLog4 = string.Format("({0},{1})\r\n", ptS.X, ptS.Y + height);
            string strLog5 = string.Format("Width = {0}\r\nHeight = {1}\r\ntotal dist = {2}\r\nArea = {3}", width, height, distTotal, area);

            this.textBox1.Text += strLog1 + strLog2 + strLog3 + strLog4 + strLog5 + "\r\n";

            // 繪製未旋轉的矩形
            DrawRectangle(ptS, ptE);

            // 詢問使用者是否要進行旋轉
            DialogResult result = MessageBox.Show("Do you want to rotate the rectangle?", "Rotate Rectangle", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                // 使用者要旋轉，獲取旋轉角度
                int rotationAngle = GetRotationAngleFromUser();

                // 繪製旋轉後的矩形
                DrawRotatedRectangle(new Point((ptS.X + ptE.X) / 2, (ptS.Y + ptE.Y) / 2), width, height, rotationAngle);
            }
        }
        private int GetRotationAngleFromUser()
        {
            int rotationAngle = 0;

            // 新增一個 Form 來顯示輸入框
            using (Form inputForm = new Form())
            {
                // 新增 TextBox
                TextBox textBox = new TextBox();
                textBox.Location = new Point(10, 10);
                inputForm.Controls.Add(textBox);

                // 新增 OK 按鈕
                Button okButton = new Button();
                okButton.Text = "OK";
                okButton.Location = new Point(10, 40);
                okButton.Click += (sender, e) =>
                {
                    // 使用 TryParse 來轉換輸入，如果失敗則使用預設值 0
                    if (int.TryParse(textBox.Text, out rotationAngle))
                        inputForm.DialogResult = DialogResult.OK;
                    else
                    {
                        MessageBox.Show("Invalid input. Using default angle (0 degrees).", "Error");
                        inputForm.DialogResult = DialogResult.Cancel;
                    }
                };
                inputForm.Controls.Add(okButton);

                // 顯示 Form 並等待使用者操作
                if (inputForm.ShowDialog() == DialogResult.Cancel)
                    rotationAngle = 0; // 使用者取消輸入，使用預設值 0
            }

            return rotationAngle;
        }
        private void DrawRectangle(Point ptS, Point ptE)//沒旋轉的
        {
            _bmp = (Bitmap)_bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);
            Pen pen = new Pen(Color.Purple, 3);
            
            int width = Math.Abs(ptE.X - ptS.X);
            int height = Math.Abs(ptE.Y - ptS.Y);

            g.DrawRectangle(pen, ptS.X, ptS.Y, width, height);

            this.pictureBox1.Image = _bmp;
        }

        private void DrawRotatedRectangle(Point center, int width, int height, float angle)
        {
            _bmp = (Bitmap)_bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);
            Pen pen = new Pen(Color.Purple, 3);

            // 計算矩形的四個角的相對座標
            PointF[] corners = new PointF[4];
            corners[0] = new PointF(-width / 2, -height / 2);
            corners[1] = new PointF(width / 2, -height / 2);
            corners[2] = new PointF(width / 2, height / 2);
            corners[3] = new PointF(-width / 2, height / 2);

            // 繪製旋轉後的矩形
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            matrix.Translate(center.X, center.Y, MatrixOrder.Append); // 先平移再旋轉
            matrix.TransformPoints(corners);

            g.DrawPolygon(pen, corners);

            // 計算旋轉後的寬度和高度
            float rotatedWidth = Math.Abs(corners[1].X - corners[0].X);
            float rotatedHeight = Math.Abs(corners[3].Y - corners[0].Y);

            // 計算總距離和面積
            float distTotal = rotatedWidth + rotatedHeight;
            float area = rotatedWidth * rotatedHeight;

            StringBuilder coordinates = new StringBuilder("Coordinate after rotation:\r\n");
            for (int i = 0; i < corners.Length; i++)
                coordinates.AppendFormat($"Corner {i + 1}: ({corners[i].X}, {corners[i].Y})\r\n");
            coordinates.AppendFormat("Width: {0}\r\n", rotatedWidth);
            coordinates.AppendFormat("Height: {0}\r\n", rotatedHeight);
            coordinates.AppendFormat("Total Distance: {0}\r\n", distTotal);
            coordinates.AppendFormat("Area: {0}\r\n", area);

            this.textBox1.Text += coordinates.ToString();

            this.pictureBox1.Image = _bmp;
        }

        //Pentagon
        private void CalculatePentagon()
        {
            if (_ptList.Count < 5) return;

            double distTotal = 0;
            double area = 0;

            StringBuilder coordinates = new StringBuilder("Coordinate: ");
            for (int i = 0; i < _ptList.Count; i++)
            {
                Point pt = (Point)_ptList[i];
                coordinates.AppendFormat("({0},{1})", pt.X, pt.Y);

                int idxE = (i + 1) % _ptList.Count;
                Point nextPt = (Point)_ptList[idxE];

                double X2 = Math.Pow(pt.X - nextPt.X, 2);
                double Y2 = Math.Pow(pt.Y - nextPt.Y, 2);
                double dist = Math.Sqrt(X2 + Y2);

                distTotal += dist;
                area += (pt.X * nextPt.Y - nextPt.X * pt.Y);

                if (i < _ptList.Count - 1)
                    coordinates.Append(" - ");
            }

            area = Math.Abs(area) / 2.0;

            string strLog1 = string.Format("total dist={0} \r\n", distTotal);
            string strLog2 = string.Format("Area = {0} \r\n\r\n", area);

            this.textBox1.Text += coordinates.ToString() + "\r\n" + strLog1 + strLog2;

            DrawPentagon(_ptList);
            _ptList.Clear();
        }

        private void DrawPentagon(ArrayList list)
        {
            if (list.Count < 5) return;

            _bmp = (Bitmap)_bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);
            Pen pen = new Pen(Color.Green, 3);

            for (int i = 0; i < list.Count; i++)
            {
                Point pt1 = (Point)list[i];
                g.DrawEllipse(pen, pt1.X, pt1.Y, 3, 3);

                int idxE = (i + 1) % list.Count;
                Point pt2 = (Point)list[idxE];
                g.DrawLine(pen, pt1, pt2);
            }

            this.pictureBox1.Image = _bmp;
        }

        //Ellipse
        private void CalculateEllipse(Point ptS, Point ptE)
        {
            double majorAxis = Math.Max(Math.Abs(ptE.X - ptS.X), Math.Abs(ptE.Y - ptS.Y));
            double minorAxis = Math.Min(Math.Abs(ptE.X - ptS.X), Math.Abs(ptE.Y - ptS.Y));

            double area = Math.PI * majorAxis * minorAxis;
            double distTotal = Math.PI * (3 * (majorAxis + minorAxis) - Math.Sqrt((3 * majorAxis + minorAxis) * (majorAxis + 3 * minorAxis)));

            // 計算橢圓焦點
            double cx = (double)(ptS.X + ptE.X) / 2;
            double cy = (double)(ptS.Y + ptE.Y) / 2;

            double f1x = cx + Math.Sqrt(Math.Pow(majorAxis, 2) - Math.Pow(minorAxis, 2));
            double f1y = cy;

            double f2x = cx - Math.Sqrt(Math.Pow(majorAxis, 2) - Math.Pow(minorAxis, 2));
            double f2y = cy;

            string strLog1 = string.Format("dist of major axis = {0} \r\n", majorAxis);
            string strLog2 = string.Format("dist of minor axis = {0} \r\n", minorAxis);
            string strLog3 = string.Format("F1: ({0},{1}) \r\n", f1x, f1y);
            string strLog4 = string.Format("F2: ({0},{1}) \r\n", f2x, f2y);
            string strLog5 = string.Format("Area = {0} \r\n", area);
            string strLog6 = string.Format("dist total = {0} \r\n\r\n", distTotal);

            this.textBox1.Text += strLog1 + strLog2 + strLog3 + strLog4 + strLog5 + strLog6;
        }
        private void DrawEllipse(Point ptS, Point ptE)
        {
            _bmp = (Bitmap)_bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);
            Pen pen = new Pen(Color.Red, 3);

            int x = Math.Min(ptS.X, ptE.X);
            int y = Math.Min(ptS.Y, ptE.Y);
            int width = Math.Abs(ptE.X - ptS.X);
            int height = Math.Abs(ptE.Y - ptS.Y);

            g.DrawEllipse(pen, x, y, width, height);

            this.pictureBox1.Image = _bmp;
        }

        //Polygon
        private void CalculatePolygon()
        {
            if (_ptList.Count < 3) return;

            // 計算 Polygon 頂點座標、周長和面積
            StringBuilder resultBuilder = new StringBuilder();
            double distTotal = 0;
            double area = 0;

            for (int i = 0; i < _ptList.Count; i++)
            {
                Point pt1 = (Point)_ptList[i];

                // 下一個點，最後一個點連接回第一個點
                Point pt2 = (Point)_ptList[(i + 1) % _ptList.Count];

                double dist = Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2));
                distTotal += dist;
                area += 0.5 * (pt1.X * pt2.Y - pt2.X * pt1.Y);

                // 將頂點座標加入結果中
                resultBuilder.AppendFormat("Corner{0}: ({1},{2}) \r\n", i + 1, pt1.X, pt1.Y);
            }

            string strLog1 = resultBuilder.ToString();
            string strLog2 = string.Format("dist total = {0} \r\n", distTotal);
            string strLog3 = string.Format("Area = {0} \r\n\r\n", Math.Abs(area));

            this.textBox1.Text += strLog1 + strLog2 + strLog3;
        }

        private void DrawPolygon(ArrayList list, Point endPoint)
        {
            _bmp = (Bitmap)_bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);
            Pen pen = new Pen(Color.Blue, 3);

            // 繪製 Polygon 邏輯，連接每個邊
            if (list.Count > 1)
            {
                g.DrawLines(pen, list.Cast<Point>().ToArray());
                g.DrawLine(pen, (Point)list[list.Count - 1], endPoint);
            }

            this.pictureBox1.Image = _bmp;
        }
        
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            string file = dlg.FileName;
            Bitmap bmp = (Bitmap)Image.FromFile(file);
            double ratioX = (double)bmp.Width / 800;
            double ratioY = (double)bmp.Height / 600;
            double scale = Math.Max(ratioX, ratioY);
            _bmpBG = new Bitmap(bmp, new Size((int)(bmp.Width / scale), (int)(bmp.Height / scale)));
            this.pictureBox1.Image = _bmpBG;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _ptList.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
