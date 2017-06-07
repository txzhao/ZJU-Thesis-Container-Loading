/*===================== Optimization.cs =====================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using BoxPacking;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace BoxPacking
{
    class Container : Control
    {
        #region 私有字段
        private Rectangle rectangle;
        private FileInfo fileOpen = null;
        private FileInfo fileSave = null;
        private int widthOfContainer = 0;
        private int heightOfContainer = 0;
        private PackingInfomation information;
        private int currentBoxWidth = 0;
        private int currentBoxHeight = 0;
        private bool canCompute = false; //是否可以进行计算
        private bool isCompute = false; //是否计算完成
        private int[,] point; //计算摆放策略时各个点的信息;0:未填放；1:已填放
        private int pointWidth = 0; //点集宽度数量（集装箱宽度）
        private int pointHeight = 0; //点集高度数量（集装箱高度）
        private double areaRatio = 0; // 摆放方案的填充率
        private int blankNum = 0; //摆放方案形成的空白数
        private int minBlankNum = 10000; //摆放方案形成的最少空白数
        private PackingStep[] packingStep; //摆放方案
        private PackingStep[] optPackingStep; //最佳摆放方案
        private bool[] isPacking; //未摆放块的序号
        private bool[] optIsPacking; //最佳方案下未摆放块的序号
        private int packingNum = 0; //最终放置的矩形块数
        private int optPackingNum;
        private int currentNum = 0; //当前放置的矩形块数
        private int[,] ExcelData; //Excel文件数据
        private int[,] boxNum; //每一站每一类箱子垒叠起来的数目（堆数）
        private static int[,] boxRest; //剩余箱子数
        private int[] _boxRest; //每一种方案下的剩余箱子数
        private int stepNum = 0; //摆放步骤数
        private int thisStepNum = 0; //当前摆放步骤数
        private int optThisStepNum = 0;
        private int[] oldBorder = new int[236]; // 每一站装货形成的边界
        private int[] newBorder = new int[236]; // 每一站装货形成的边界（新）
        private int[] optNewBorder = new int[236];
        private int currentX; //显示矩形时矩形位置
        private int currentY; //显示矩形时矩形位置
        private int[] boxNumSum; //显示矩形时每一类矩形的数目；
        private int[,] boxRestSum; //显示矩形时每一站每一类箱子垒裸起来的数目
        private Color[] boxColor = { Color.Yellow, Color.Green, Color.Red, Color.Pink };
        private Graphics graphics;
        private Timer timer = new Timer();
        private Bitmap bitmap;
        private ProgressBar progressBar;
        private int proportion = 0;
        private Angle[] angle; // 保存角信息
        private int angleHead; // 角的头地址
        private int angleEnd; //角的尾地址
        private int connState = 0;
        private int[,] boxSize = { { 53, 29 }, { 53, 23 }, { 43, 21 }, { 35, 19 }, { 29, 17 }, { 26, 15 }, 
                                 { 23, 13 }, { 21, 11 }, { 20, 11 }, { 18, 10 }, { 15, 9 }, { 13, 8 } };
        private int[] boxTimes = { 6, 8, 8, 10, 12, 13, 14, 17, 17, 19, 21, 26 };

        private struct Box
        {
            public int Width;
            public int Height;
        }

        private struct Angle
        {
            public int pre;
            public int after;
            public int type;
        }

        public struct PackingStep
        {
            public int SerialNumber;
            public int BoxNo;
            public int XCoordinate;
            public int YCoordinate;
            public int type;  //是否横放：1：横放；0：正放
        }
        #endregion 私有字段

        public PackingInfomation PackingInformation
        {
            set
            {
                information = value;
            }
        }

        public Rectangle Rectangle
        {
            set
            {
                rectangle = value;
            }
            get
            {
                return rectangle;
            }
        }

        public Timer Timer
        {
            get
            {
                return timer;
            }
        }

        public Bitmap Bitmap
        {
            set
            {
                bitmap = value;
            }
        }

        public ProgressBar ProgressBar
        {
            set
            {
                progressBar = value;
            }
        }

        public int Proportion
        {
            get
            {
                return proportion;
            }
            set
            {
                proportion = value;
            }
        }

        //打开文件，读取箱子尺寸数据。
        public void OpenFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "e:\\";
            dlg.Filter = "Excel files(*.xls)|*.xls|Excel files(*.xlsx)|*.xlsx";
            dlg.FilterIndex = 0;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.fileOpen = new FileInfo(@dlg.FileName);
                MessageBox.Show("文件打开成功", "系统信息");
            }
        }

        //保存箱子摆放信息
        public void SaveFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = "e:\\";
            dlg.Filter = "txt files (*.txt)|*.txt|Excel files(*.xls)|*.xls";
            dlg.FilterIndex = 0;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(dlg.FileName);
                sw.WriteLine("=================================================================================");
                sw.WriteLine();
                sw.WriteLine("面积利用率：" + Math.Round(areaRatio * 100, 2) + "%");
                sw.WriteLine();
                sw.WriteLine("=================================================================================");
                sw.WriteLine();
                sw.WriteLine("已放入箱子:");
                sw.WriteLine("| 序号 | 箱子号 | 宽度 | 高度 | 坐标(X) | 坐标(Y) | 是否转置(0:不转置；1：转置) |");
                for (int i = 0; i < stepNum; i++)
                {
                    sw.Write("  ");
                    if (i < 9) sw.Write(Convert.ToString(i + 1) + "      ");
                    else if (i < 99 && i >= 9) sw.Write(Convert.ToString(i + 1) + "     ");
                    else sw.Write(Convert.ToString(i + 1) + "    ");

                    if (packingStep[i].BoxNo < 9) sw.Write(Convert.ToString(packingStep[i].BoxNo + 1) + "        ");
                    else if (packingStep[i].BoxNo < 99 && packingStep[i].BoxNo >= 9) sw.Write(Convert.ToString(packingStep[i].BoxNo + 1) + "       ");
                    else sw.Write(Convert.ToString(packingStep[i].BoxNo + 1) + "      ");

                    if (boxSize[packingStep[i].BoxNo, 0] < 10) sw.Write(Convert.ToString(boxSize[packingStep[i].BoxNo, 0]) + "      ");
                    else if (boxSize[packingStep[i].BoxNo, 0] < 100 && boxSize[packingStep[i].BoxNo, 0] >= 10) sw.Write(Convert.ToString(boxSize[packingStep[i].BoxNo, 0]) + "     ");
                    else sw.Write(Convert.ToString(boxSize[packingStep[i].BoxNo, 0]) + "    ");

                    if (boxSize[packingStep[i].BoxNo, 1] < 10) sw.Write(Convert.ToString(boxSize[packingStep[i].BoxNo, 1]) + "      ");
                    else if (boxSize[packingStep[i].BoxNo, 1] < 100 && boxSize[packingStep[i].BoxNo, 1] >= 10) sw.Write(Convert.ToString(boxSize[packingStep[i].BoxNo, 1]) + "     ");
                    else sw.Write(Convert.ToString(boxSize[packingStep[i].BoxNo, 1]) + "    ");

                    if (packingStep[i].XCoordinate < 10) sw.Write(Convert.ToString(packingStep[i].XCoordinate) + "         ");
                    else if (packingStep[i].XCoordinate < 100 && packingStep[i].XCoordinate >= 10) sw.Write(Convert.ToString(packingStep[i].XCoordinate) + "        ");
                    else sw.Write(Convert.ToString(packingStep[i].XCoordinate) + "       ");

                    if (packingStep[i].YCoordinate < 10) sw.Write(Convert.ToString(packingStep[i].YCoordinate) + "         ");
                    else if (packingStep[i].YCoordinate < 100 && packingStep[i].YCoordinate >= 10) sw.Write(Convert.ToString(packingStep[i].YCoordinate) + "        ");
                    else sw.Write(Convert.ToString(packingStep[i].YCoordinate) + "       ");

                    sw.WriteLine(Convert.ToString(packingStep[i].type));
                }
                sw.WriteLine();
                sw.WriteLine("=================================================================================");
                sw.WriteLine();
                sw.WriteLine("未放入箱子:");
                sw.WriteLine("| 序号 | 箱子号 | 批次号 | 宽度 | 高度 | 数量 |");
                int index = 0;
                for (int i = 0; i < boxRest.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < boxRest.GetLength(1) - 1; j++)
                    {
                        if (boxRest[i, j] == 0) continue;
                        index++;
                        
                        sw.Write("  ");
                        if (index < 10) sw.Write(Convert.ToString(index) + "      ");
                        else if (index < 100 && index >= 10) sw.Write(Convert.ToString(index) + "     ");
                        else sw.Write(Convert.ToString(index) + "    ");

                        if (j < 9) sw.Write(Convert.ToString(j + 1) + "        ");
                        else if (j < 99 && j >= 9) sw.Write(Convert.ToString(j + 1) + "       ");
                        else sw.Write(Convert.ToString(j + 1) + "      ");

                        if (i < 9) sw.Write(Convert.ToString(i + 1) + "        ");
                        else if (i < 99 && i >= 9) sw.Write(Convert.ToString(i + 1) + "       ");
                        else sw.Write(Convert.ToString(i + 1) + "      ");

                        if (boxSize[j + 1, 0] < 10) sw.Write(Convert.ToString(boxSize[j + 1, 0]) + "      ");
                        else if (boxSize[j + 1, 0] < 100 && boxSize[j + 1, 0] >= 10) sw.Write(Convert.ToString(boxSize[j + 1, 0]) + "     ");
                        else sw.Write(Convert.ToString(boxSize[j + 1, 0]) + "    ");

                        if (boxSize[j + 1, 1] < 10) sw.Write(Convert.ToString(boxSize[j + 1, 1]) + "      ");
                        else if (boxSize[j + 1, 1] < 100 && boxSize[j + 1, 1] >= 10) sw.Write(Convert.ToString(boxSize[j + 1, 1]) + "     ");
                        else sw.Write(Convert.ToString(boxSize[j + 1, 1]) + "    ");

                        sw.WriteLine(Convert.ToString(boxRest[i, j]));
                    }   
                }
                sw.Dispose();
            }
        }

        //计算箱子摆放策略
        public void StartCompute()
        {
            if (this.fileOpen == null)
            {
                MessageBox.Show("no file open", "error");
                return;
            }
            if (!canCompute)
            {
                MessageBox.Show("未输入容器大小", "error");
                return;
            }
            Clear();

            //progressBar.Value = 0;
            //progressBar.Visible = true;

            #region 读取文件数据到box
            ReadFile readFile = new ReadFile();
            Array Data = readFile.ReadXls(fileOpen.DirectoryName + fileOpen.Name, 1);//读取test.xlsx的第一个sheet表
            int fileLength = Data.Length;
            ExcelData = new int[fileLength / 12, 12];
            boxNum = new int[fileLength / 12, 12];
            boxRest = new int[fileLength / 12, 12];
            _boxRest = new int[12];
            for (int i = 0; i < fileLength / 12; i++)
                for (int j = 0; j < 12; j++)
                {
                    ExcelData[i, j] = Convert.ToInt32(Data.GetValue(i + 1, j + 1));
                    if (ExcelData[i, j] % boxTimes[j] == 0)
                        boxNum[i, j] = ExcelData[i, j] / boxTimes[j];
                    else boxNum[i, j] = ExcelData[i, j] / boxTimes[j] + 1;
                }
            #endregion

            int[] sum = new int[fileLength / 12];       // sum - 求各站点的堆数和，Sum - 求所有堆数和
            int Sum = 0;
            for (int i = 0; i < fileLength / 12; i++)
                for (int j = 0; j < 12; j++)
                {
                    sum[i] += boxNum[i, j];
                    Sum += boxNum[i, j];
                }
            //progressBar.Maximum = 2 * box.Length * (box.Length - 1);
            pointWidth = 234;
            pointHeight = 589;
            point = new int[pointHeight + 2, pointWidth + 2];
            packingStep = new PackingStep[Sum];
            optPackingStep = new PackingStep[Sum];

            #region point初始化、oldBorder初始化
            for (int i = 0; i < pointHeight + 2; i++)
            {
                point[i, 0] = point[i, pointWidth + 1] = 1;
            }
            for (int j = 0; j < pointWidth + 2; j++)
            {
                point[0, j] = point[pointHeight + 1, j] = 1;
            }
            oldBorder[0] = oldBorder[235] = 590;
            #endregion

            int BlankNumSum = 0;
            int PointSum = 0;
            for (int SN = 0; SN < fileLength / 12; SN++)
            {
                minBlankNum = 10000;
                for (int i = 0; i < 12; i++)
                {
                    OptimalPacking(i, SN, sum[SN]);
                    if (blankNum < minBlankNum)
                    {
                        minBlankNum = blankNum;
                        for (int j = 0; j < thisStepNum; j++)
                        {
                            optPackingStep[stepNum + j].BoxNo = packingStep[j].BoxNo;
                            optPackingStep[stepNum + j].XCoordinate = packingStep[j].XCoordinate;
                            optPackingStep[stepNum + j].YCoordinate = packingStep[j].YCoordinate;
                            optPackingStep[stepNum + j].type = packingStep[j].type;
                            optPackingStep[stepNum + j].SerialNumber = SN;
                        }
                        for (int j = 0; j < 12; j++)
                        {
                            boxRest[SN, j] = _boxRest[j];
                        }
                        optThisStepNum = thisStepNum;
                        for (int j = 0; j < oldBorder.Length; j++)
                            optNewBorder[j] = newBorder[j];
                    }
                }
                stepNum += optThisStepNum;
                BlankNumSum += minBlankNum;
                for (int j = 1; j < pointWidth + 1; j++)
                {
                    for (int i = oldBorder[j]; i <= optNewBorder[j]; i++) point[i, j] = 1;
                    oldBorder[j] = optNewBorder[j];
                }
            }

            for (int i = 1; i < pointWidth + 1; i++) PointSum += oldBorder[i];
            areaRatio = 1 - ((double)BlankNumSum / (double)PointSum);
            isCompute = true;
            //归并法排序
            packingStep = SortList(optPackingStep, stepNum);
            //progressBar.Value = progressBar.Maximum;
            updateBoxInfo();
            MessageBox.Show("计算完成，可以开始绘图", "系统信息");
            //progressBar.Visible = false;
        }

        //进行最优摆放方案的计算；index：第一块box放置的序号；type：是否横放。
        // sn:第几站货物； sum该站货物总数
        public void OptimalPacking(int index, int sn, int sum)
        {
            #region point初始化设置
            for (int j = 1; j < pointWidth + 1; j++)
                for (int i = oldBorder[j] + 1; i < pointHeight + 1; i++)
                {
                    point[i, j] = 0;
                }
            #endregion

            #region angle初始设置
            int currentAngle;
            angle = new Angle[1000000];
            // 搜索角的头地址
            int HeadAddress = 1;
            while (oldBorder[HeadAddress] == 590)
            {
                HeadAddress++;
            }
            angleHead = HeadAddress + (oldBorder[HeadAddress] + 1) * 1000;  //此处angleHead为一个六位数，前三位保存y值，后三位保存x值
            angle[angleHead].pre = 0;
            angle[angleHead].type = 0;
            currentAngle = angleHead;
            for (int j = HeadAddress + 1; j < pointWidth + 1; j++)
            {
                if (oldBorder[j] < oldBorder[j - 1])
                {
                    angle[currentAngle].after = j + (oldBorder[j] + 1) * 1000;
                    angle[j + (oldBorder[j] + 1) * 1000].pre = currentAngle;
                    angle[j + (oldBorder[j] + 1) * 1000].type = 0;
                    currentAngle = j + (oldBorder[j] + 1) * 1000;
                    continue;
                }
                if (oldBorder[j] < oldBorder[j + 1])
                {
                    angle[currentAngle].after = j + (oldBorder[j] + 1) * 1000;
                    angle[j + (oldBorder[j] + 1) * 1000].pre = currentAngle;
                    angle[j + (oldBorder[j] + 1) * 1000].type = 1;
                    currentAngle = j + (oldBorder[j] + 1) * 1000;
                    continue;
                }
            }
            angle[currentAngle].after = 0;
            angleEnd = currentAngle;
            #endregion

            #region _boxRest初始设置
            for (int i = 0; i < 12; i++)
            {
                _boxRest[i] = boxNum[sn, i];
            }
            #endregion

            for (int num = 0; num < sum; num++)
            {
                bool packing = false;
                double _optDegree = 100000000; //最佳矩形在最佳位置的穴度
                int _optArea = 0; //最佳矩形在最佳位置的贴边数
                int width = 0; //最佳矩形的宽度
                int height = 0; //最佳矩形的长度
                int angleIndex = angleHead;
                //搜索每个角
                currentAngle = angleHead;
                while (currentAngle != 0)
                {
                    int optNo = -1; //最佳矩形的型号
                    double optDegree = 10000000; //最佳矩形的穴度
                    int optArea = 0; //最佳矩形的面积
                    int optType = 0; //最佳矩形的摆放类型
                    for (int No = 0; No < 12; No++)
                    {
                        if (_boxRest[No] == 0)
                            continue;
                        if (num == 0 && No != index)
                            continue;
                        int heightValue = currentAngle / 1000;
                        int widthValue = currentAngle % 1000;

                        // 计算正放矩形
                        if (angle[currentAngle].type == 1)
                        {
                            widthValue = widthValue - boxSize[No, 0] + 1;
                        }
                        if (ValidPlace(heightValue, widthValue, boxSize[No, 0], boxSize[No, 1]))
                        {
                            if (Degree(heightValue, widthValue, boxSize[No, 0], boxSize[No, 1]) < optDegree)
                            {
                                optDegree = Degree(heightValue, widthValue, boxSize[No, 0], boxSize[No, 1]);
                                optArea = boxSize[No, 0] * boxSize[No, 1];
                                optNo = No;
                                optType = 0;
                            }
                            else if (Degree(heightValue, widthValue, boxSize[No, 0], boxSize[No, 1]) == optDegree && (boxSize[No, 0] * boxSize[No, 1] > optArea))
                            {
                                //optDegree = Degree(heightValue, widthValue, boxSize[No, 0], boxSize[No, 1]);
                                optArea = boxSize[No, 0] * boxSize[No, 1];
                                optNo = No;
                                optType = 0;
                            }
                        }

                        //计算横放矩形
                        widthValue = currentAngle % 1000;
                        if (angle[currentAngle].type == 1)
                        {
                            widthValue = widthValue - boxSize[No, 1] + 1;
                        }
                        if (ValidPlace(heightValue, widthValue, boxSize[No, 1], boxSize[No, 0]))
                        {
                            if (Degree(heightValue, widthValue, boxSize[No, 1], boxSize[No, 0]) < optDegree)
                            {
                                optDegree = Degree(heightValue, widthValue, boxSize[No, 1], boxSize[No, 0]);
                                optArea = boxSize[No, 0] * boxSize[No, 1];
                                optNo = No;
                                optType = 1;
                            }
                            else if (Degree(heightValue, widthValue, boxSize[No, 1], boxSize[No, 0]) == optDegree && (boxSize[No, 1] * boxSize[No, 0] > optArea))
                            {
                                //optDegree = Degree(heightValue, widthValue, boxSize[No, 1], boxSize[No, 0]);
                                optArea = boxSize[No, 0] * boxSize[No, 1];
                                optNo = No;
                                optType = 1;
                            }
                        }

                        //搜索最优矩形
                        if (optNo != -1)
                        {
                            packing = true;
                            if (optDegree < _optDegree || (optDegree == _optDegree && optArea > _optArea))
                            {
                                _optDegree = optDegree;
                                _optArea = optArea;
                                packingStep[num].BoxNo = No;
                                packingStep[num].type = optType;
                                packingStep[num].XCoordinate = currentAngle / 1000;
                                packingStep[num].YCoordinate = currentAngle % 1000;     //YCoordinate是到y轴的距离
                                if (angle[currentAngle].type == 1)
                                {
                                    if (optType == 0)
                                    {
                                        packingStep[num].YCoordinate -= boxSize[No, 0] - 1;
                                    }
                                    else
                                        packingStep[num].YCoordinate -= boxSize[No, 1] - 1;
                                }
                            }
                        }
                    }
                    currentAngle = angle[currentAngle].after;
                }

                #region 放置第num块矩形
                if (!packing)
                {
                    thisStepNum = num;
                    break;
                }
                if (num == sum - 1)
                {
                    thisStepNum = sum;
                }
                _boxRest[packingStep[num].BoxNo]--;
                width = boxSize[packingStep[num].BoxNo, 0];
                height = boxSize[packingStep[num].BoxNo, 1];
                if (packingStep[num].type == 1)
                {
                    int flag = width;
                    width = height;
                    height = flag;
                }
                for (int i = packingStep[num].XCoordinate; i < packingStep[num].XCoordinate + height; i++)
                {
                    for (int j = packingStep[num].YCoordinate; j < packingStep[num].YCoordinate + width; j++)
                    {
                        point[i, j] = 1;
                    }
                }
                #endregion

                #region 修改角
                //先删除原来的角
                if (point[packingStep[num].XCoordinate, packingStep[num].YCoordinate - 1] == 1)
                {
                    int angleNum = packingStep[num].XCoordinate * 1000 + packingStep[num].YCoordinate - 1;
                    angle[angle[angleNum].pre].after = angle[angleNum].after;
                    angle[angleNum].pre = angle[angleNum].after = angle[angleNum].type = 0;
                }
                if (point[packingStep[num].XCoordinate, packingStep[num].YCoordinate + width] == 1)
                {
                    int angleNum = packingStep[num].XCoordinate * 1000 + packingStep[num].YCoordinate + width;
                    angle[angle[angleNum].pre].after = angle[angleNum].after;
                    angle[angleNum].pre = angle[angleNum].after = angle[angleNum].type = 0;
                }
                //再添加新生成的角
                for (int i = packingStep[num].XCoordinate; i < packingStep[num].XCoordinate + height; i++)
                {
                    if (point[i, packingStep[num].YCoordinate - 1] == 0 && point[i - 1, packingStep[num].YCoordinate - 1] == 1)
                    {
                        int _angle = i * 1000 + packingStep[num].YCoordinate - 1;
                        angle[angleEnd].after = _angle;
                        angle[_angle].pre = angleEnd;
                        angle[_angle].after = 0;
                        angle[_angle].type = 1;
                        angleEnd = _angle;
                    }
                    if (point[i, packingStep[num].YCoordinate + width] == 0 && point[i - 1, packingStep[num].YCoordinate + width] == 1)
                    {
                        int _angle = i * 1000 + packingStep[num].YCoordinate + width;
                        angle[angleEnd].after = _angle;
                        angle[_angle].pre = angleEnd;
                        angle[_angle].after = 0;
                        angle[_angle].type = 0;
                        angleEnd = _angle;
                    }
                }
                for (int j = packingStep[num].YCoordinate; j < packingStep[num].YCoordinate + width; j++)
                {
                    if (point[packingStep[num].XCoordinate + height, j] == 0 && point[packingStep[num].XCoordinate + height, j - 1] == 1)
                    {
                        int _angle = (packingStep[num].XCoordinate + height) * 1000 + j;
                        angle[angleEnd].after = _angle;
                        angle[_angle].pre = angleEnd;
                        angle[_angle].after = 0;
                        angle[_angle].type = 0;
                        angleEnd = _angle;
                        continue;
                    }
                    if (point[packingStep[num].XCoordinate + height, j] == 0 && point[packingStep[num].XCoordinate + height, j + 1] == 1)
                    {
                        int _angle = (packingStep[num].XCoordinate + height) * 1000 + j;
                        angle[angleEnd].after = _angle;
                        angle[_angle].pre = angleEnd;
                        angle[_angle].after = 0;
                        angle[_angle].type = 1;
                        angleEnd = _angle;
                        continue;
                    }
                }
                #endregion

                // progressBar.Value++;
            }
            newBorder = BorderCompute();
            blankNum = BlankNumCompute();
        }

        //计算新边界
        public int[] BorderCompute()
        {
            int[] a = new int[236];
            for (int j = 0; j < 236; j++)
            {
                for (int i = 589; i > -1; i--)
                {
                    if (point[i, j] == 1)
                    {
                        a[j] = i;
                        if (a[j] == 589)
                            a[j]++;
                        break;
                    }
                }
            }
            return a;
        }

        //计算空白数
        public int BlankNumCompute()
        {
            int blanknum = 0;
            for (int j = 1; j < 235; j++)
                for (int i = oldBorder[j] + 1; i < newBorder[j]; i++)
                    if (point[i, j] == 0)
                        blanknum++;
            return blanknum;
        }

        //判断矩形是否可以放置此处
        public bool ValidPlace(int i, int j, int width, int height)
        {
            if (i < 1 || j < 1 || i + height - 1 > pointHeight || j + width - 1 > pointWidth)
                return false;
            for (int i0 = i; i0 < i + height; i0++)
                if (point[i0, j] == 1 || point[i0, j + width - 1] == 1)
                    return false;
            for (int j0 = j; j0 < j + width; j0++)
            {
                if (point[i, j0] == 1 || point[i + height - 1, j0] == 1)
                    return false;
            }
            return true;
        }

        //计算穴度
        public double Degree(int x0, int y0, int width, int height)
        {
            int[] distance = { 0, 0, 0, 0 };
            int shortDistance;
            int[] valid = new int[2];
            int num = 0;
            int no = 0;
            double degree;
            int flag;
            // 计算四个边与其他物块的距离
            flag = 0;
            for (int i = x0 - 1; i > 0; i--)
            {
                for (int j = y0; j < j + width; j++)
                {
                    if (point[i, j] == 1)
                    {
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                    distance[0]++;
                else break;
            }
            flag = 0;
            for (int i = x0 + height; i < heightOfContainer + 1; i++)
            {
                for (int j = y0; j < j + width; j++)
                {
                    if (point[i, j] == 1)
                    {
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                    distance[1]++;
                else break;
            }
            flag = 0;
            for (int j = y0 - 1; j > 0; j--)
            {
                for (int i = x0; i < i + height; i++)
                {
                    if (point[i, j] == 1)
                    {
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                    distance[2]++;
                else break;
            }
            for (int j = y0 + width; j < widthOfContainer + 1; j++)
            {
                for (int i = x0; i < i + height; i++)
                {
                    if (point[i, j] == 1)
                    {
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                    distance[3]++;
                else break;
            }
            // 搜索四边最短距离（除去两个贴边）
            for (int i = 0; i < 4; i++)
            {
                if (distance[i] == 0)
                    num++;
                else valid[no++] = distance[i];
            }
            if (num > 2)
                shortDistance = 0;
            else if (valid[0] < valid[1])
                shortDistance = valid[0];
            else shortDistance = valid[1];
            // 计算穴度
            //degree = (double)(shortDistance * shortDistance * 1.0) / (double)(width * height);
            degree = shortDistance;
            return degree;
        }

        // 归并法排序
        public PackingStep[] SortList(PackingStep[] packingStep, int length)
        {
            if (length == 1 || length == 0)
                return packingStep;
            int a, b;
            PackingStep[] step1, step2;
            a = length / 2;
            b = length - a;
            step1 = new PackingStep[a];
            step2 = new PackingStep[b];
            for (int i = 0; i < a; i++)
            {
                step1[i] = packingStep[i];
                step2[i] = packingStep[i + a];
            }
            step2[b - 1] = packingStep[length - 1];
            step1 = SortList(step1, a);
            step2 = SortList(step2, b);
            return Merge(step1, step2);
        }

        public PackingStep[] Merge(PackingStep[] step1, PackingStep[] step2)
        {
            int length, length1, length2, i;
            PackingStep[] step;
            length = step1.Length + step2.Length;
            length1 = 0;
            length2 = 0;
            i = 0;
            step = new PackingStep[length];
            while (length1 != step1.Length && length2 != step2.Length)
            {
                if (step1[length1].SerialNumber < step2[length2].SerialNumber)
                {
                    step[i++] = step1[length1++];
                }
                else if (step1[length1].SerialNumber == step2[length2].SerialNumber && step1[length1].XCoordinate < step2[length2].XCoordinate)
                {
                    step[i++] = step1[length1++];
                }
                else if (step1[length1].SerialNumber == step2[length2].SerialNumber && step1[length1].XCoordinate == step2[length2].XCoordinate && step1[length1].YCoordinate < step2[length2].YCoordinate)
                {
                    step[i++] = step1[length1++];
                }
                else
                {
                    step[i++] = step2[length2++];
                }
            }
            if (length1 == step1.Length)
            {
                while (length2 != step2.Length)
                {
                    step[i++] = step2[length2++];
                }
            }
            else if (length2 == step2.Length)
            {
                while (length1 != step1.Length)
                {
                    step[i++] = step1[length1++];
                }
            }
            return step;
        }

        public void Display()
        {
            Graphics graphics = Graphics.FromImage(bitmap);
            //progressBar.Visible = false;
            currentNum = 0;
            if (!isCompute)
            {
                MessageBox.Show("未完成计算，不能绘图", "error");
                return;
            }
            //清除痕迹
            Clear();
            currentX = packingStep[currentNum].YCoordinate - 1;
            currentY = 5;
            currentBoxWidth = boxSize[packingStep[currentNum].BoxNo, 0];
            currentBoxHeight = boxSize[packingStep[currentNum].BoxNo, 1];
            if (packingStep[currentNum].type == 1)
            {
                int x;
                x = currentBoxWidth;
                currentBoxWidth = currentBoxHeight;
                currentBoxHeight = x;
            }
            timer.Enabled = true;
        }
        
        public void TimerTick(object sender, EventArgs e)
        {
            information.Update(this);
            if (currentY == 5)
            {
                Rectangle rect = new Rectangle(currentX, currentY, currentBoxWidth, currentBoxHeight);
                graphics.FillRectangle(new SolidBrush(boxColor[currentNum % 4]), rect);
                Invalidate(rect);
                
            }
            else
            {
                Rectangle rect1 = new Rectangle(currentX, currentY - 1, currentBoxWidth, 1);
                Rectangle rect2 = new Rectangle(currentX, currentY + currentBoxHeight - 1, currentBoxWidth, 1);
                graphics.FillRectangle(new SolidBrush(boxColor[currentNum % 4]), rect2);
                Invalidate(rect2);
                if (currentY - 1 < 60)
                    graphics.FillRectangle(new SolidBrush(SystemColors.Control), rect1);
                else
                    graphics.FillRectangle(new SolidBrush(this.BackColor), rect1);
                Invalidate(rect1);
            }
            if (currentY == this.Height - packingStep[currentNum].XCoordinate + 1 - currentBoxHeight)
            {
                updateBoxInfo(packingStep[currentNum].BoxNo, packingStep[currentNum].SerialNumber);
                currentNum++;
                if (currentNum == stepNum)
                {
                    timer.Enabled = false;
                    MessageBox.Show("绘图结束", "系统信息");
                    return;
                }
                currentX = packingStep[currentNum].YCoordinate - 1;
                currentY = 5;
                currentBoxWidth = boxSize[packingStep[currentNum].BoxNo, 0];
                currentBoxHeight = boxSize[packingStep[currentNum].BoxNo, 1];
                if (packingStep[currentNum].type == 1)
                {
                    int x;
                    x = currentBoxWidth;
                    currentBoxWidth = currentBoxHeight;
                    currentBoxHeight = x;
                }
            }
            else
            {
                currentY ++;
            }
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Enabled = false;
            }
        }

        public void Continue()
        {
            if (isCompute)
            {
                timer.Enabled = true;
            }
        }

        public void Clear()
        {
            if (graphics != null)
            {
                Rectangle rect = new Rectangle(0, proportion * 60, widthOfContainer * proportion, heightOfContainer * proportion);
                graphics.FillRectangle(new SolidBrush(this.BackColor), rect);
                Invalidate();
            }
        }

        public void Result()
        {
            if (!isCompute)
                return;
            string s = "";
            s += "面积利用率：" + Math.Round(areaRatio * 100, 2) + "%";
            s += System.Environment.NewLine;
            s += System.Environment.NewLine;
            s += "未放入箱子:";
            s += System.Environment.NewLine;
            s += "| 序号 | 箱子号 | 批次号 | 宽度 | 高度 | 数量 |";
            s += System.Environment.NewLine;

            int index = 0;
            for (int i = 0; i < boxRest.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < boxRest.GetLength(1) - 1; j++)
                {
                    if (boxRest[i, j] == 0) continue;
                    index++;

                    s += "  ";
                    if (index < 10) s += Convert.ToString(index) + "        ";
                    else if (index < 100 && index >= 10) s += Convert.ToString(index) + "      ";
                    else s += Convert.ToString(index) + "     ";

                    if (j < 9) s += Convert.ToString(j + 1) + "          ";
                    else if (j < 99 && j >= 9) s += Convert.ToString(j + 1) + "        ";
                    else s += Convert.ToString(j + 1) + "       ";

                    if (i < 9) s += Convert.ToString(i + 1) + "         ";
                    else if (i < 99 && i >= 9) s += Convert.ToString(i + 1) + "       ";
                    else s += Convert.ToString(i + 1) + "      ";

                    if (boxSize[j + 1, 0] < 10) s += Convert.ToString(boxSize[j + 1, 0]) + "        ";
                    else if (boxSize[j + 1, 0] < 100 && boxSize[j + 1, 0] >= 10) s += Convert.ToString(boxSize[j + 1, 0]) + "      ";
                    else s += Convert.ToString(boxSize[j + 1, 0]) + "     ";

                    if (boxSize[j + 1, 1] < 10) s += Convert.ToString(boxSize[j + 1, 1]) + "       ";
                    else if (boxSize[j + 1, 1] < 100 && boxSize[j + 1, 1] >= 10) s += Convert.ToString(boxSize[j + 1, 1]) + "     ";
                    else s += Convert.ToString(boxSize[j + 1, 1]) + "    ";

                    s += Convert.ToString(boxRest[i, j]);
                    s += System.Environment.NewLine;
                }
            }
            MessageBox.Show(s, "运行结果");
        }

        public void Login()
        {
            string ip = "10.15.197.65";
            string port = "21390";
            string db = "test1";
            string connStr = "server=" + ip + "," + port + ";database=" + db + ";Connection Timeout=100;uid=sa;pwd=123456";
            SqlConnection mySqlConnection = new SqlConnection(connStr);
            try
            {
                mySqlConnection.Open();
                mySqlConnection.Close();
                MessageBox.Show("数据库连接成功！");
                connState = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接失败！原因如下： " + ex.Message);
                connState = 0;
            }
        }

        public void CleanSheet()
        {
            string ip = "10.15.197.65";
            string port = "21390";
            string db = "test1";
            string connStr = "server=" + ip + "," + port + ";database=" + db + ";uid=sa;pwd=123456";
            SqlConnection CleanConnection = new SqlConnection(connStr);
            CleanConnection.Open();
            string cleanStr = "truncate Table PackingInfo";
            SqlCommand CleanCmd = new SqlCommand(cleanStr, CleanConnection);
            CleanCmd.ExecuteNonQuery();
            CleanConnection.Close();
        }

        public void AddData()
        {
            string ip = "10.15.197.65";
            string port = "21390";
            string db = "test1";
            string connStr = "server=" + ip + "," + port + ";database=" + db + ";Connection Timeout=100;uid=sa;pwd=123456";
            if (connState == 1)
            {
                CleanSheet();
                SqlConnection mySqlConnection = new SqlConnection(connStr);
                mySqlConnection.Open();
                for (int i = 0; i < stepNum; i++)
                {
                    string str = "insert into PackingInfo(Index,BoxNo,Width,Height,CoordinateX,CoordinateY,Pattern) values('";
                    str += Convert.ToString(i + 1) + "," + Convert.ToString(packingStep[i].BoxNo + 1) + "," + Convert.ToString(boxSize[packingStep[i].BoxNo, 0]);
                    str += "," + Convert.ToString(boxSize[packingStep[i].BoxNo, 1]) + "," + Convert.ToString(packingStep[i].XCoordinate) + ",";
                    str += Convert.ToString(packingStep[i].YCoordinate) + "," + Convert.ToString(packingStep[i].type);
                    SqlCommand SqlCmd = new SqlCommand(str, mySqlConnection);
                    SqlCmd.ExecuteNonQuery();
                }
                mySqlConnection.Close();
            }
            else MessageBox.Show("未连接数据库！");
        }

        public int[] SetSize()
        {
            canCompute = true;
            int[] size = new int[2];
            this.widthOfContainer = 234;
            this.heightOfContainer = 589;
            this.Width = this.widthOfContainer * proportion + 100;
            this.Height = this.heightOfContainer * proportion + Proportion * 60;
            size[0] = this.widthOfContainer;
            size[1] = this.heightOfContainer;
            return size;
        }

        public void DrawFram(Color color)
        {
            graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(new SolidBrush(color), this.widthOfContainer * proportion, 0, 100, this.Height);
            graphics.FillRectangle(new SolidBrush(color), 0, 0, this.Width, Proportion * 60);
            Invalidate();
        }

        public void UpdateInfo(Graphics graphics)
        {
            StringFormat vStringFormat = new StringFormat();
            vStringFormat.Alignment = StringAlignment.Near;
            Rectangle rect1 = new Rectangle(100, 10, 30, 20);
            Rectangle rect2 = new Rectangle(100, 30, 30, 20);
            Rectangle rect3 = new Rectangle(100, 50, 30, 20);
            Rectangle rect4 = new Rectangle(100, 70, 30, 20);
            Rectangle rect5 = new Rectangle(100, 90, 30, 20);
            Rectangle rect6 = new Rectangle(100, 110, 30, 15);
            Rectangle rect = new Rectangle(0, 0, information.Width, information.Height);
            graphics.DrawString(Convert.ToString(packingStep[currentNum].SerialNumber + 1), new Font("宋体", 10f), Brushes.Black, rect1, vStringFormat);
            graphics.DrawString(Convert.ToString(packingStep[currentNum].BoxNo + 1), new Font("宋体", 10f), Brushes.Black, rect2, vStringFormat);
            graphics.DrawString(Convert.ToString(currentBoxWidth), new Font("宋体", 10f), Brushes.Black, rect3, vStringFormat);
            graphics.DrawString(Convert.ToString(currentBoxHeight), new Font("宋体", 10f), Brushes.Black, rect4, vStringFormat);
            graphics.DrawString(Convert.ToString(packingStep[currentNum].XCoordinate), new Font("宋体", 10f), Brushes.Black, rect5, vStringFormat);
            graphics.DrawString(Convert.ToString(packingStep[currentNum].YCoordinate), new Font("宋体", 10f), Brushes.Black, rect6, vStringFormat);
        }

        public void updateBoxInfo()
        {
            boxNumSum = new int[12];
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < ExcelData.Length / 12; j++)
                    boxNumSum[i] += ExcelData[j, i];
            }
            boxRestSum = new int[ExcelData.Length / 12, 12];
            for (int i = 0; i < ExcelData.Length / 12; i++)
            {
                for (int j = 0; j < 12; j++)
                    boxRestSum[i, j] = boxNum[i, j];
            }
            graphics = Graphics.FromImage(bitmap);
            int h = this.Height - 589;
            for (int i = 1; i < 13; i++)
            {
                string s = Convert.ToString(i);
                if (i < 10)
                    s += " :";
                else
                    s += ":";
                if (boxNumSum[i - 1] >= 100)
                    s += Convert.ToString(boxNumSum[i - 1]);
                else if (boxNumSum[i - 1] >= 10)
                    s += " " + Convert.ToString(boxNumSum[i - 1]);
                else
                    s += "  " + Convert.ToString(boxNumSum[i - 1]);
                Rectangle rect1 = new Rectangle(this.Width - 96, h + 10, 95, 30);
                graphics.FillRectangle(new SolidBrush(SystemColors.Control), rect1);
                graphics.DrawString(s, new Font("宋体", 20f), Brushes.Black, rect1);
                Rectangle rect = new Rectangle(this.Width - 96, h, 95, 49);
                graphics.DrawRectangle(new Pen(Color.Green), rect);
                Invalidate(rect);
                h = h + 49;
            }
        }

        public void updateBoxInfo(int No, int Index)
        {
            string s = Convert.ToString(No + 1);
            if (No + 1 < 10)
                s += " :";
            else
                s += ":";
            if (boxRestSum[Index, No] > 1 || ExcelData[Index, No] % boxTimes[No] == 0)
            {
                boxNumSum[No] -= boxTimes[No];
            }
            else
            {
                boxNumSum[No] -= ExcelData[Index, No] % boxTimes[No];
            }
            if (boxNumSum[No] >= 100)
                s += boxNumSum[No];
            else if (boxNumSum[No] >= 10)
                s += " " + boxNumSum[No];
            else
                s += "  " + boxNumSum[No];
            graphics = Graphics.FromImage(bitmap);
            int h = this.Height - 589 + No * 49;
            Rectangle rect1 = new Rectangle(this.Width - 96, h + 10, 95, 30);
            graphics.FillRectangle(new SolidBrush(SystemColors.Control), rect1);
            graphics.DrawString(s, new Font("宋体", 20f), Brushes.Black, rect1);
            Rectangle rect = new Rectangle(this.Width - 96, h, 95, 49);
            graphics.DrawRectangle(new Pen(Color.Green), rect);

            Invalidate(rect);
        }

        public void DrawBoxInfo()
        {
            graphics = Graphics.FromImage(bitmap);
            int h = this.Height - 589;
            for (int i = 1; i < 13; i++)
            {
                Rectangle rect = new Rectangle(this.Width - 96, h, 95, 49);
                graphics.DrawRectangle(new Pen(Color.Green), rect);
                Rectangle rect1 = new Rectangle(this.Width - 96, h + 10, 95, 30);
                if (i < 10)
                    graphics.DrawString(Convert.ToString(i) + " :  0", new Font("宋体", 20f), Brushes.Black, rect1);
                else
                    graphics.DrawString(Convert.ToString(i) + ":  0", new Font("宋体", 20f), Brushes.Black, rect1);

                h = h + 49;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (bitmap != null)
            {
                e.Graphics.DrawImage(bitmap, 0, 0);
            }
        }
    }

    class PackingInfomation : Control
    {
        #region 私有字段
        private Bitmap bitmapRec;
        private Bitmap bitmapInfo;
        #endregion 私有字段

        public void DrawRect(int[] size)
        {
            bitmapRec = new Bitmap(this.Width, this.Height);
            Graphics AGraphics = Graphics.FromImage(bitmapRec);
            GraphicsPath path = new GraphicsPath();
            int r = 1;
            path.AddArc(0 + r, 0 + r, 20, 20, 180, 90);
            path.AddArc(this.Width - 20 - r, 0 + r, 20, 20, 270, 90);
            path.AddArc(this.Width - 20 - r, this.Height - 20 - r, 20, 20, 0, 90);
            path.AddArc(0 + r, this.Height - 20 - r, 20, 20, 90, 90);
            path.CloseFigure();
            AGraphics.DrawPath(new Pen(Color.Gray), path); 
            DrawInfo(size);
            Invalidate();
        }

        public void DrawInfo(int[] size)
        {
            bitmapInfo = new Bitmap(this.Width, this.Height);
            Graphics AGraphics = Graphics.FromImage(bitmapInfo);
            StringFormat vStringFormat = new StringFormat();
            vStringFormat.Alignment = StringAlignment.Near;
            Rectangle rect1 = new Rectangle(10, 10, 80, 20);
            Rectangle rect2 = new Rectangle(100, 10, 30, 20);
            Rectangle rect3 = new Rectangle(10, 30, 80, 20);
            Rectangle rect4 = new Rectangle(100, 30, 30, 20);
            Rectangle rect5 = new Rectangle(10, 50, 80, 20);
            Rectangle rect6 = new Rectangle(100, 50, 30, 20);
            Rectangle rect7 = new Rectangle(10, 70, 80, 20);
            Rectangle rect8 = new Rectangle(100, 70, 30, 20);
            Rectangle rect9 = new Rectangle(10, 90, 80, 20);
            Rectangle rect10 = new Rectangle(100, 90, 30, 20);
            Rectangle rect11 = new Rectangle(10, 110, 80, 15);
            Rectangle rect12 = new Rectangle(100, 110, 30, 15);
            AGraphics.DrawString("当前批次:", new Font("宋体", 10f), Brushes.Black, rect1, vStringFormat);
            AGraphics.DrawString("-", new Font("宋体", 10f), Brushes.Black, rect2, vStringFormat);
            AGraphics.DrawString("箱子型号:", new Font("宋体", 10f), Brushes.Black, rect3, vStringFormat);
            AGraphics.DrawString("-", new Font("宋体", 10f), Brushes.Black, rect4, vStringFormat);
            AGraphics.DrawString("箱子宽度:", new Font("宋体", 10f), Brushes.Black, rect5, vStringFormat);
            AGraphics.DrawString("-", new Font("宋体", 10f), Brushes.Black, rect6, vStringFormat);
            AGraphics.DrawString("箱子高度:", new Font("宋体", 10f), Brushes.Black, rect7, vStringFormat);
            AGraphics.DrawString("-", new Font("宋体", 10f), Brushes.Black, rect8, vStringFormat);
            AGraphics.DrawString("坐标(X):", new Font("宋体", 10f), Brushes.Black, rect9, vStringFormat);
            AGraphics.DrawString("-", new Font("宋体", 10f), Brushes.Black, rect10, vStringFormat);
            AGraphics.DrawString("坐标(Y):", new Font("宋体", 10f), Brushes.Black, rect11, vStringFormat);
            AGraphics.DrawString("-", new Font("宋体", 10f), Brushes.Black, rect12, vStringFormat);

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (bitmapRec != null)
            {
                e.Graphics.DrawImage(bitmapRec, 0, 0);
            }
            if (bitmapInfo != null)
            {
                e.Graphics.DrawImage(bitmapInfo, 0, 0);
            }
        }

        public void Update(Container container)
        {
            if (container == null)
            {
                return;
            }
            Clear();
            container.UpdateInfo(Graphics.FromImage(bitmapInfo));
            Rectangle rect = new Rectangle(10, 10, this.Width - 20, this.Height - 20);
            Invalidate(rect);
            Refresh();
        }

        public void Clear()
        {
            Graphics graphics = Graphics.FromImage(bitmapInfo);
            Rectangle rect1 = new Rectangle(100, 10, 30, 20);
            Rectangle rect2 = new Rectangle(100, 30, 30, 20);
            Rectangle rect3 = new Rectangle(100, 50, 30, 20);
            Rectangle rect4 = new Rectangle(100, 70, 30, 20);
            Rectangle rect5 = new Rectangle(100, 90, 30, 20);
            Rectangle rect6 = new Rectangle(100, 110, 30, 15);
            graphics.FillRectangle(new SolidBrush(BackColor), rect1);
            graphics.FillRectangle(new SolidBrush(BackColor), rect2);
            graphics.FillRectangle(new SolidBrush(BackColor), rect3);
            graphics.FillRectangle(new SolidBrush(BackColor), rect4);
            graphics.FillRectangle(new SolidBrush(BackColor), rect5);
            graphics.FillRectangle(new SolidBrush(BackColor), rect6);
        }

    }
}
