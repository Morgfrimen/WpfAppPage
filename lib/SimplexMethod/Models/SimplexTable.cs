using System;

namespace SimplexMethod.Models
{
    //TODO: Валидацитя входных параметров
    /// <summary>
    /// Симплекс таблица
    /// </summary>
    internal class SimplexTable : IMethod
    {
        private double[] B;
        private double[,] A;


        #region Настройки
        /// <summary>
        /// Валидное ли значение таблицы
        /// </summary>
        private bool _validValue = false;

        /// <summary>
        /// Настройка включения проверки входных параметров
        /// </summary>
        private bool _runValid = false; 
        #endregion

        #region Свойства описания таблицы
        /// <summary>
        /// Шаг итерации
        /// </summary>
        public uint StepIteration { get; set; }

        /// <summary>
        /// Базисные значения
        /// </summary>
        public string[] Bazis { get; set; }

        /// <summary>
        /// Вектор решений ограничений НЕРАВЕНСТВ
        /// </summary>
        public double[] BVector { get; set; }

        /// <summary>
        /// Вектор решение ограничений РАВЕНСТВ
        /// </summary>
        public double[] BeqVector { get; set; }

        /// <summary>
        /// Матрица или мектор коэффициентов ограничений-НЕРАВЕНСТВ
        /// </summary>
        public double[,] AMatrix { get; set; }

        /// <summary>
        /// Матрица или мектор коэффициентов ограничений-РАВЕНСТВ
        /// </summary>
        public double[,] AeqMatrix { get; set; }

        /// <summary>
        /// Верхний потолок (размерность массива не важна, важен порядок)
        /// </summary>
        public double[] MaxValue { get; set; }

        /// <summary>
        /// Нижний потолок значений (размерность не важна, важен порядок)
        /// </summary>
        public double[] MinValues { get; set; }

        /// <summary>
        /// Коэффициенты целевой функции
        /// </summary>
        public double[] ZVector { get; set; }

        /// <summary>
        /// Значение целевой функции
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Коэффициенты дополнительной целевой функции
        /// </summary>
        public double[] WVector { get; set; }

        /// <summary>
        /// Значение новой целевой функции
        /// </summary>
        public double W { get; set; }

        /// <summary>
        /// Delta при расчетах
        /// </summary>
        public double[] Delta { get; set; }
        #endregion

        #region Get методы
        /// <summary>
        /// Валидные ли данные таблицы
        /// </summary>
        public bool GetValidValue() => this._validValue;

        /// <summary>
        /// Выставлена ли проверка валидности входных параметров
        /// </summary>
        public bool GetSettingValid() => this._runValid;

        /// <summary>
        /// Получаем вектор
        /// </summary>
        /// <returns></returns>
        public double[] GetB() => this.B;

        /// <summary>
        /// Получаем матрицу А
        /// </summary>
        /// <returns></returns>
        public double[,] GetA() => this.A;
        #endregion

        #region Set методы
        /// <summary>
        /// Выставление настроки проверки валидности входных данных
        /// </summary>
        /// <param name="settings"></param>
        public void SetSettingValid(bool settings) => this._runValid = settings;

        /// <summary>
        /// Запись целековлй матрицы принеобходимости
        /// </summary>
        /// <param name="A"></param>
        public void SetAMatrix (double[,] A) => this.A = A;

        /// <summary>
        /// Запись целекого вектора при необходимости
        /// </summary>
        /// <param name="B"></param>
        public void SetBVector(double[] B) => this.B = B;

        #endregion

        /// <inheritdoc />
        public string StartValue { get; set; }

        public bool Validate { get => _runValid; set => _runValid = value; }


        public object GetResult() => this;
    }
}
