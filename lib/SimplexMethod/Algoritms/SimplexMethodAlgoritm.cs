using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using SimplexMethod.Models;

namespace SimplexMethod.Algoritms
{
    /// <summary>
    /// Класс с алгоритмом решения по универсальному симплекс методу
    /// </summary>
    internal class SimplexMethodAlgoritm : IAlgoritm
    {
        private SimplexTable _simplexTable;
        private bool IsMatrix = false;
        internal SimplexMethodAlgoritm(SimplexTable simplexTable)
        {
            _simplexTable = simplexTable;
            IsMatrix = _simplexTable.AMatrix is double[,];
        }

        
        public void RunAlgoritm()
        {
            double[] BVector = _simplexTable.BVector;
            double[,] AMatrix = _simplexTable.AMatrix;

            double[] BeqVector = _simplexTable.BeqVector;
            double[,] AeqMatrix = _simplexTable.AeqMatrix;

            //Меняем знак всего ограничения, если он отрицательный
            if (BVector != null && AMatrix != null)
            {
                ReverseSign(ref BVector, ref AMatrix); 
            }
            if (BeqVector !=null && AeqMatrix != null)
            {
                ReverseSign(ref BeqVector, ref AeqMatrix); 
            }

            //Добавляем базисные уравнения
            if (AMatrix != null && AeqMatrix != null)
            {
                AddBazisInMatrixA(ref AMatrix , ref AeqMatrix);
            }

            //Соединяем B c Beq в один массив и A с Aeq в одну матрицу
            double[] B = GetBVector(BVector, BeqVector);
            double[,] A = GetAMatrix(AMatrix, AeqMatrix);

            //Собираем дефолтный массив базисов
            string[] newStringsBazis = new string[B.Length];
            for (int row = 0; row < B.Length; row++)
            {
                newStringsBazis[row] = $"X{row + 1}";
            }
            _simplexTable.Bazis = newStringsBazis;

            //Если число базисных переменных равно числу ограничений
            if (_simplexTable.ZVector.Length == _simplexTable.BVector.Length)
                UniversalSimplexMethod(true);

            //Добавляем в равенства исскуственные базисные переменные
            AddBazisInMatrixA(ref A,AMatrix.GetLength(0)-1,AeqMatrix.GetLength(1));
            
            //Получаем новую целевую функцию
            CreateWVector(ref AeqMatrix,ref BeqVector);

            //Запуск универсального симплекс метода
            _simplexTable.SetAMatrix(A);
            _simplexTable.SetBVector(B);

            UniversalSimplexMethod(false);
        }

        private int FindMinIndexInVector(double[] value)
        {
            double min = value.Min();
            int minIndex = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == min)
                    minIndex = i;
            }

            return minIndex;
        }


        private void UniversalSimplexMethod(bool IsSimple)
        {
            double Z = _simplexTable.Z;
            double[] C = _simplexTable.ZVector;

            double[] d = new double[_simplexTable.WVector.Length];
            double W = _simplexTable.W;

            double[] B = _simplexTable.GetB();
            double[,] A = _simplexTable.GetA();

            if (!IsSimple)
            {
                //Собрал вектор новой целевой функции
                for (int rowIndex = 0; rowIndex < d.Length; rowIndex++)
                    d[rowIndex] = _simplexTable.WVector[rowIndex];

                //Собираю вектор С под размер d
                double[] newC = new double[d.Length];
                for (int rowIndex = 0; rowIndex < C.Length; rowIndex++)
                    newC[rowIndex] = C[rowIndex];
            }
            else
            {
                //Собрал вектор целевой функции
                for (int rowIndex = 0; rowIndex < C.Length; rowIndex++)
                    C[rowIndex] = _simplexTable.ZVector[rowIndex];
            }

            //Находим базисное значение(колонка)
            double dBaz = d.Min();
            int indexBaz = 0;
            if (!IsSimple)
                indexBaz = FindMinIndexInVector(d);
            else
                indexBaz = FindMinIndexInVector(C);

            while (_simplexTable.StepIteration <= 25)
            {
                //Если зацикливание
                if (_simplexTable.StepIteration >= 25)
                    throw new Exception(message: "Зацикливание в универсальном симплексе");

                if (!IsSimple)
                {
                    //Проверка условия минимального d
                    if ((dBaz >= 0) && (W >= 0.5) && (W <= -0.5))
                        UniversalSimplexMethod(true); 
                }
                else
                {
                    _simplexTable.ZVector = C;
                    _simplexTable.Z = Z;
                    _simplexTable.WVector = d;
                    _simplexTable.W = W;
                    _simplexTable.SetAMatrix(A);
                    _simplexTable.SetBVector(B);
                    return;
                }

                //Опеределние опорной строки
                double[] promegResult = new double[B.Length];
                for (int rowIndex = 0; rowIndex < promegResult.Length; rowIndex++)
                {
                    if (A[rowIndex , indexBaz] > 0)
                        promegResult[rowIndex] = B[rowIndex] / A[rowIndex , indexBaz];
                }

                //Собираем дельту
                _simplexTable.Delta = promegResult;

                double BOpor = promegResult.Min();
                int indexOpor = FindMinIndexInVector(value: promegResult);

                //Заносим запись в базисный список
                _simplexTable.Bazis[indexBaz] = $"X{indexOpor+1}";

                //Пересчет B
                for (int rowIndex = 0; rowIndex < B.Length; rowIndex++)
                {
                    if (rowIndex != indexOpor)
                        B[rowIndex] -= BOpor * A[rowIndex , indexBaz];
                }

                //Пересчет А опорного
                for (int columnIndex = 0; columnIndex < A.GetLength(dimension: 1); columnIndex++)
                {
                    if (columnIndex != indexBaz)
                        A[indexOpor , columnIndex] /= A[indexOpor , indexBaz];
                }

                //Пересчет А
                for (int rowIndex = 0; rowIndex < A.GetLength(dimension: 0); rowIndex++)
                {
                    if (rowIndex != indexOpor)
                        for (int columnIndex = 0; columnIndex < A.GetLength(dimension: 1); columnIndex++)
                        {
                            if (columnIndex != indexBaz)
                                A[rowIndex , columnIndex] -= A[indexOpor , columnIndex] * A[rowIndex , indexBaz];
                        }
                }

                //Пересчет векторов d и C
                for (int rowIndex = 0; rowIndex < C.Length; rowIndex++)
                {
                    if (rowIndex != indexBaz)
                    {
                        C[rowIndex] -= A[indexOpor , rowIndex] * C[indexBaz];
                        if (!IsSimple)
                        {
                            d[rowIndex] -= A[indexOpor, rowIndex] * d[indexBaz]; 
                        }
                    }
                }

                //Пересчет Z и W
                Z += C[indexBaz] * BOpor;
                if(!IsSimple)
                    W -= d[indexBaz] * BOpor;

                //Дефолтные значения
                C[indexBaz] = 0;
                if (!IsSimple)
                    d[indexBaz] = 0;
                A[indexOpor , indexBaz] = 1;
                for (int rowIndex = 0; rowIndex < A.GetLength(dimension: 0); rowIndex++)
                {
                    if (rowIndex != indexOpor)
                        A[rowIndex , indexBaz] = 0;
                }

                _simplexTable.StepIteration++;
                _simplexTable.WVector = d;
                _simplexTable.W = W;
            }
        }




        private void CreateWVector(ref double[,] Aeq, ref double[] Beq)
        {
            double[,] resultMatrix = new double[Aeq.GetLength(0),Aeq.GetLength(1)-Aeq.GetLength(0)];

            //Перенём значения и сделал обратный знак
            for (int rowIndex = 0; rowIndex < resultMatrix.GetLength(0); rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < resultMatrix.GetLength(1); columnIndex++)
                {
                    resultMatrix[rowIndex , columnIndex] = Aeq[rowIndex,columnIndex] * -1;
                }
            }

            double[] WVector = new double[resultMatrix.GetLength(1) + 1];
            for (int column = 0; column < resultMatrix.GetLength(1); column++)
            {
                for (int rowIndex = 0; rowIndex < resultMatrix.GetLength(0); rowIndex++)
                {
                    WVector[column] += resultMatrix[rowIndex , column];
                }
            }

            WVector[^0] = Beq.Sum();

            _simplexTable.WVector = WVector;
        }

        private double[] GetBVector (double[] b , double[] beq)
        {
            if ((b == null) && (beq != null))
                return beq;
            if ((beq == null) && (b != null))
                return b;
            if ((b == null) && (beq == null))
                throw new Exception(message: "Матрица B и Beq is null");
            double[] newVector = new double[b.Length + beq.Length];

            for (int i = 0; i < (b.Length + beq.Length); i++)
            {
                if (i < b.Length)
                    newVector[i] = b[i];
                else
                    newVector[i] = beq[i];
            }

            return newVector;
        }

        private double[,] GetAMatrix(double[,] a, double[,] aeq)
        {
            if ((a == null) && (aeq == null))
                throw new Exception(message: "Матрица А и Aeq равны null");

            //Необходимая валидация
            if (a.GetLength(dimension: 1) != aeq.GetLength(dimension: 1))
                throw new Exception(message: "Размерности КОЛОНОК матриц не равны!");

            if ((a == null) && (aeq != null))
                return aeq;
            if ((a != null) && (aeq == null))
                return a;

            int lev1 = a.GetLength(dimension: 1);
            double[,] newMatrix = new double[a.GetLength(0) + aeq.GetLength(0) , lev1];

            //Пернос в новую матрицу первую матрицу
            for (int rowIndex = 0; rowIndex < a.GetLength(0); rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < lev1; columnIndex++)
                {
                    newMatrix[rowIndex , columnIndex] = a[rowIndex , columnIndex];
                }
            }

            //Пересон в новую матрицу вторую матрицу
            for (int rowIndex = a.GetLength(0); rowIndex < newMatrix.GetLength(0); rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < lev1; columnIndex++)
                {
                    newMatrix[rowIndex, columnIndex] = aeq[rowIndex - a.GetLength(0), columnIndex];
                }
            }

            return newMatrix;
        }



        private void ReverseSign(ref double[] bVector, ref double[,] aMatrix)
        {
            for (int rowIndex = 0; rowIndex < bVector.Length; rowIndex++)
            {
                double value = bVector[rowIndex];
                if (value < 0)
                {
                    bVector[rowIndex] *= -1;

                    for (int columIndex = 0; columIndex < aMatrix.GetLength(1); columIndex++)
                    {
                        aMatrix[rowIndex, columIndex] *= -1;
                    }
                }
            }
        }

        private void AddBazisInMatrixA (ref double[,] A , int rowIndexStart , int countNewBazis)
        {
            double[,] resultA = new double[A.GetLength(0) , A.GetLength(1) + countNewBazis];

            //Перенос значений из старой матрицы в новую
            for (int rowIndex = 0; rowIndex < A.GetLength(0); rowIndex++)
            {
                for (int columnsIndex = 0; columnsIndex < A.GetLength(1); columnsIndex++)
                {
                    resultA[rowIndex , columnsIndex] = A[rowIndex , columnsIndex];
                }
            }

            //Добавление базиса
            for (int rowIndex = rowIndexStart; rowIndex < A.GetLength(0); rowIndex++)
            {
                for (int columnIndex = A.GetLength(1); columnIndex < resultA.GetLength(1); columnIndex++)
                {
                    resultA[rowIndex , columnIndex] = 1;
                }
            }

            A = resultA;
        }

        private void AddBazisInMatrixA(ref double[,] AMatrix, ref double[,] AeqMatrix)
        {
            double[,] oldMatrix = AMatrix;

            int lev0 = oldMatrix.GetLength(dimension: 0);
            int lev1 = oldMatrix.GetLength(dimension: 1);
            double[,] newAMatrix = new double[lev0 , lev1 + lev0];
            for (int rowIndex = 0; rowIndex < lev0; rowIndex++)
            {
                for (int columIndex = 0; columIndex < lev1; columIndex++)
                    newAMatrix[rowIndex , columIndex] = oldMatrix[rowIndex , columIndex];
            }

            for (int rowIndex = 0; rowIndex < lev0; rowIndex++)
                newAMatrix[rowIndex , lev1 + rowIndex] = 1;

            AMatrix = newAMatrix;

            //Меняем длину для Aeq
            oldMatrix = AeqMatrix;
            lev0 = oldMatrix.GetLength(dimension: 0);
            lev1 = oldMatrix.GetLength(dimension: 1);

            double[,] newAeqMatrix = new double[lev0 , lev1 + lev0];

            for (int rowIndex = 0; rowIndex < lev0; rowIndex++)
            {
                for (int columIndex = 0; columIndex < lev1; columIndex++)
                    newAeqMatrix[rowIndex , columIndex] = oldMatrix[rowIndex , columIndex];
            }

            AeqMatrix = newAeqMatrix;

        }

#region Методы Get

        internal SimplexTable GetSimplexTable() => _simplexTable;

#endregion


    }
}
