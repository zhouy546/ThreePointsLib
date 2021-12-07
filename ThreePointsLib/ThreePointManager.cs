using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ThreePointsLib
{


    public class pointManager
    {

        public const int pointDiv = 3;

        public const float angleThreshold = 10f;

        public const float MaxPerimeter = 1000;


        public static List<Node> GetNODES(List<Vector3> touchPositions, float Re_angleA = 45,float Re_angleB = 90,float Re_angleC = 45)
        {
            return getNode(getTriPositions(touchPositions, Re_angleA, Re_angleB, Re_angleC));
        }


        static List<Node> getNode(List<Vector3[]> positionGroup)
        {
            List<Node> Nodes = new List<Node>();

            foreach (var positionArray in positionGroup)
            {
                Nodes.Add(new Node(positionArray));
            }

            return Nodes;
        }


        static List<Vector3[]> getTriPositions(List<Vector3> positions, float CheckAngleA, float CheckAngleB, float CheckAngleC)
        {
            List<Vector3[]> Tri = new List<Vector3[]>();
            if (positions.Count >= 3)
            {
                IEnumerable<int[]> unms = test(positions.Count);

                Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();

                foreach (var item in unms)
                {
                    int A = item[0];
                    int B = item[1];
                    int C = item[2];

                    Vector2 positionA = positions[A];
                    Vector2 positionB = positions[B];
                    Vector2 positionC = positions[C];

                    float angleA = utility.getAngle(positionA, positionB, positionC);
                    float angleB = utility.getAngle(positionB, positionA, positionC);
                    float angleC = utility.getAngle(positionC, positionB, positionA);


                    float TRIPerimeter = (positionC - positionA).magnitude + (positionB - positionA).magnitude + (positionC - positionB).magnitude;
                    if (checkAngle(angleA, angleB, angleC, CheckAngleA, CheckAngleB, CheckAngleC) && TRIPerimeter < MaxPerimeter)
                    {

                        Vector3[] array = new Vector3[3];
                        for (int i = 0; i < item.Length; i++)
                        {
                            array[i] = positions[item[i]];
                        }

                        Tri.Add(array);
                    }
                }

            }
            return Tri;
        }


        static float[] SortAngle(float a, float b, float c)
        {
            List<float> listfloat = new List<float>();
            listfloat.Add(a);
            listfloat.Add(b);
            listfloat.Add(c);
            listfloat.Sort();
            return listfloat.ToArray();
        }

        static bool  checkAngle(float a, float b, float c, float checkAngelA, float checkAngelB, float checkAngelC)
        {

            float[] touchAngel = SortAngle(a, b, c);
            float[] CheckAngel = SortAngle(checkAngelA, checkAngelB, checkAngelC);

            float tempa = CheckAngel[0] + angleThreshold;
            float temp_a = CheckAngel[0] - angleThreshold;

            float tempb = CheckAngel[1] + angleThreshold;
            float temp_b = CheckAngel[1] - angleThreshold;

            float tempc = CheckAngel[2] + angleThreshold;
            float temp_c = CheckAngel[2] - angleThreshold;

            if (temp_a < touchAngel[0] && touchAngel[0] < tempa && temp_b < touchAngel[1] && touchAngel[1] < tempb && temp_c < touchAngel[2] && touchAngel[2] < tempc)
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        //5选3
        static IEnumerable<int[]> test(int PointCount)
        {
            int n = pointDiv;
            List<int> temp = new List<int>();

            for (int i = 0; i < PointCount; i++)
            {
                temp.Add(i);
            }
            int[] are = temp.ToArray();

            var result = are.Select(x => new int[] { x });
            for (int i = 0; i < n - 1; i++)
            {
                result = result.SelectMany(x => are.Where(y => y.CompareTo(x.First()) < 0).Select(y => new int[] { y }.Concat(x).ToArray()));
            }

            return result;
        }

    }

    public class Node
    {


        public Vector3[] touchPosition;

        public Vector3 centerPos;

        public List<float> angles = new List<float>();

        public float Rotangle;

        public Node()
        {

        }

        public Node(Vector3[] _positions)
        {
            touchPosition = _positions;

            centerPos = getCenterPos(touchPosition);

            float angleA = utility.getAngle(_positions[0], _positions[1], _positions[2]);
            float angleB = utility.getAngle(_positions[1], _positions[0], _positions[2]);
            float angleC = utility.getAngle(_positions[2], _positions[1], _positions[0]);

            Vector3 bigAngelPoint = Vector3.zero;

            angles.Add(angleA);

            angles.Add(angleB);

            angles.Add(angleC);

            angles.Sort();

            if (angles[2] == angleA)
            {
                bigAngelPoint = _positions[0];
            }
            else if (angles[2] == angleB)
            {
                bigAngelPoint = _positions[1];
            }
            else if (angles[2] == angleC)
            {
                bigAngelPoint = _positions[2];
            }


            Rotangle = getAngel(centerPos, bigAngelPoint);
        }

        float getAngel(Vector3 centerpos, Vector3 TopPos)
        {




            float rad = Mathf.Atan2((TopPos.x - centerpos.x), (TopPos.y - centerpos.y)); //弧度  0.9272952180016122
            float angele = rad * (180 / Mathf.PI); //角度  53.13010235415598
            return angele;
        }

        Vector3 getCenterPos(Vector3[] finger)
        {
            float x1, x2, x3, y1, y2, y3, X, Y;


            x1 = finger[0].x;
            y1 = finger[0].y;

            x2 = finger[1].x;
            y2 = finger[1].y;

            x3 = finger[2].x;
            y3 = finger[2].y;

            X = (x1 + x2 + x3) / 3;
            Y = (y1 + y2 + y3) / 3;

            return new Vector3(X, Y, 0);
        }
    }





    public static class utility
    {
        public static float pi180 = 180 / Mathf.PI;
        public static float getAngle(Vector2 a, Vector2 b, Vector2 c)
        {
            var _cos1 = getCos(a.x, a.y, b.x, b.y, c.x, c.y);//第一个点为顶点的角的角度的余弦值

            return Mathf.Acos(_cos1) * pi180;
        }

        //获得三个点构成的三角形的 第一个点所在的角度的余弦值
        public static float getCos(float point1_x, float point1_y, float point2_x, float point2_y, float point3_x, float point3_y)
        {
            var length1_2 = getLength(point1_x, point1_y, point2_x, point2_y);//获取第一个点与第2个点的距离
            var length1_3 = getLength(point1_x, point1_y, point3_x, point3_y);
            var length2_3 = getLength(point2_x, point2_y, point3_x, point3_y);

            float res = (Mathf.Pow(length1_2, 2) + Mathf.Pow(length1_3, 2) - Mathf.Pow(length2_3, 2)) / (length1_2 * length1_3 * 2);//cosA=(pow(b,2)+pow(c,2)-pow(a,2))/2*b*c

            return res;
        }

        //获取坐标轴内两个点间的距离
        public static float getLength(float point1_x, float point1_y, float point2_x, float point2_y)
        {
            var diff_x = Mathf.Abs(point2_x - point1_x);
            var diff_y = Mathf.Abs(point2_y - point1_y);

            var length_pow = Mathf.Pow(diff_x, 2) + Mathf.Pow(diff_y, 2);//两个点在 横纵坐标的差值与两点间的直线 构成直角三角形。length_pow等于该距离的平方

            return Mathf.Sqrt(length_pow);
        }

    }
}
