using OxyPlot;
using OxyPlot.Axes;

namespace LinearCodingEmulator
{
    internal class LinearCoder
    {
        public OxyPlot.Series.LineSeries PointList;
        public LinearAxis linearAxisY;
        public LinearAxis linearAxisX;
        public PlotModel model;
        private string Mode;
        private static int[] CodedMessage;

        public bool Code(string Alg, string Message)
        {
            foreach (var t in Message)
            {
                if (t != '0' && t != '1') return false;
            }
            Mode = Alg;
            switch (Alg)
            {
                case "NRZ Uni":
                    return NRZUni(Message);

                case "NRZ Bi":
                    return NRZBi(Message);

                case "AMI":
                    return AMI(Message);

                case "B3Z":
                    return B3Z(Message);

                case "B6Z":
                    return B6Z(Message);

                case "HDB3":
                    return HDB3(Message);

                default:
                    return false;
            }
        }

        private void FillPoinList(double MaxDelta = 1)
        {
            model = new PlotModel();

            linearAxisY = new LinearAxis();
            linearAxisY.MajorGridlineStyle = LineStyle.Dot;
            linearAxisY.Position = AxisPosition.Bottom;
            linearAxisY.IsZoomEnabled = false;
            linearAxisY.IsPanEnabled = false;
            model.Axes.Add(linearAxisY);

            linearAxisX = new LinearAxis();
            linearAxisX.MajorGridlineStyle = LineStyle.Solid;
            linearAxisX.MajorGridlineThickness = 2;
            linearAxisX.CropGridlines = true;
            linearAxisX.IsZoomEnabled = false;
            linearAxisX.IsPanEnabled = false;
            linearAxisX.AxislineColor = OxyColors.Black;
            model.Axes.Add(linearAxisX);

            PointList = new OxyPlot.Series.LineSeries();
            PointList.Color = OxyColors.Red;

            model.Series.Add(PointList);

            int idx1 = 0;
            PointList.Points.Add(new DataPoint(idx1, CodedMessage[0] * MaxDelta));
            for (int idx0 = 0; idx0 < CodedMessage.Length - 1; idx0++)
            {
                idx1++;
                PointList.Points.Add(new DataPoint(idx1, CodedMessage[idx0] * MaxDelta));

                if (CodedMessage[idx0] != CodedMessage[idx0 + 1])
                {
                    PointList.Points.Add(new DataPoint(idx1, CodedMessage[idx0 + 1] * MaxDelta));
                }
            }
            PointList.Points.Add(new DataPoint(idx1 + 1, CodedMessage[CodedMessage.Length - 1] * MaxDelta));
        }

        private bool NRZUni(string Message)
        {
            CodedMessage = new int[Message.Length];
            for (int idx = 0; idx < CodedMessage.Length; idx++)
            {
                CodedMessage[idx] = int.Parse(Message[idx].ToString());
            }
            FillPoinList();
            return true;
        }

        private bool AMI(string Message)
        {
            CodedMessage = new int[Message.Length];
            int sign = 1;
            int value = 0;
            for (int idx = 0; idx < CodedMessage.Length; idx++)
            {
                value = int.Parse(Message[idx].ToString());
                if (value == 1)
                {
                    CodedMessage[idx] = sign;
                    sign *= -1;
                }
                else CodedMessage[idx] = 0;
            }
            FillPoinList(0.5);
            return true;
        }

        private bool NRZBi(string Message)
        {
            CodedMessage = new int[Message.Length];
            for (int idx = 0; idx < CodedMessage.Length; idx++)
            {
                CodedMessage[idx] = (int.Parse(Message[idx].ToString()) == 0 ? -1 : 1);
            }
            FillPoinList(0.5);
            return true;
        }

        private bool B3Z(string Message)
        {
            CodedMessage = new int[Message.Length];
            int sign = 1;
            int value = 0;
            int FZeroIdx = 0;
            int SequenceLenght = 0;
            bool isZeroSequence = false;
            for (int idx = 0; idx < CodedMessage.Length; idx++)
            {
                value = int.Parse(Message[idx].ToString());
                if (value == 1)
                {
                    if (isZeroSequence)
                    {
                        SequenceLenght = idx - FZeroIdx;
                        SequenceLenght = SequenceLenght / 3;
                        if (SequenceLenght % 2 == 0)//B0V
                        {
                            for (int idx1 = FZeroIdx; idx1 < FZeroIdx + SequenceLenght * 3;)
                            {
                                CodedMessage[idx1] = sign;//B
                                CodedMessage[idx1 + 2] = sign;//V
                                sign *= -1;
                                idx1 += 3;
                            }
                        }
                        else//00V
                        {
                            for (int idx1 = FZeroIdx; idx1 < FZeroIdx + SequenceLenght * 3;)
                            {
                                CodedMessage[idx1 + 2] = -sign;//V
                                idx1 += 3;
                            }
                        }
                        isZeroSequence = false;
                    }

                    CodedMessage[idx] = sign;
                    sign *= -1;
                }
                else
                {
                    if (isZeroSequence == false)
                    {
                        FZeroIdx = idx;
                        isZeroSequence = true;
                    }
                    CodedMessage[idx] = value;
                }
            }
            FillPoinList(0.5);
            return true;
        }

        private bool B6Z(string Message)
        {
            CodedMessage = new int[Message.Length];
            int sign = 1;
            int value = 0;
            int FZeroIdx = 0;
            int SequenceLenght = 0;
            bool isZeroSequence = false;
            for (int idx = 0; idx < CodedMessage.Length; idx++)
            {
                value = int.Parse(Message[idx].ToString());
                if (value == 1)
                {
                    if (isZeroSequence)//0VB0VB
                    {
                        SequenceLenght = idx - FZeroIdx;
                        SequenceLenght = SequenceLenght / 6;

                        for (int idx1 = FZeroIdx; idx1 < FZeroIdx + SequenceLenght * 6;)
                        {
                            CodedMessage[idx1 + 1] = -sign;//V
                            CodedMessage[idx1 + 2] = sign;//B

                            CodedMessage[idx1 + 4] = sign;//V
                            CodedMessage[idx1 + 5] = -sign;//B
                            idx1 += 6;
                        }

                        isZeroSequence = false;
                    }

                    CodedMessage[idx] = sign;
                    sign *= -1;
                }
                else
                {
                    if (isZeroSequence == false)
                    {
                        FZeroIdx = idx;
                        isZeroSequence = true;
                    }
                    CodedMessage[idx] = value;
                }
            }
            FillPoinList(0.5);
            return true;
        }

        private bool HDB3(string Message)
        {
            CodedMessage = new int[Message.Length];
            int sign = 1;
            int value = 0;
            int FZeroIdx = 0;
            int SequenceLenght = 0;
            bool isZeroSequence = false;
            for (int idx = 0; idx < CodedMessage.Length; idx++)
            {
                value = int.Parse(Message[idx].ToString());
                if (value == 1)
                {
                    if (isZeroSequence)
                    {
                        SequenceLenght = idx - FZeroIdx;
                        SequenceLenght = SequenceLenght / 4;

                        for (int idx1 = FZeroIdx; idx1 < FZeroIdx + SequenceLenght * 4;)
                        {
                            CodedMessage[idx1 + 3] = -sign;
                            idx1 += 4;
                        }

                        isZeroSequence = false;
                    }

                    CodedMessage[idx] = sign;
                    sign *= -1;
                }
                else
                {
                    if (isZeroSequence == false)
                    {
                        FZeroIdx = idx;
                        isZeroSequence = true;
                    }
                    CodedMessage[idx] = value;
                }
            }
            FillPoinList(0.5);
            return true;
        }
    }
}