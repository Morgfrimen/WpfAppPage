using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SimplexMethod.Models;

namespace Methods.ParserMethod
{

    internal class ParserSimplexMethod: IParser
    {
        SimplexTable simplexMethod = new SimplexTable();

        //Формат данных: Z,C,B,A,Beq,Aeq,Min,Max
        private const string patternVector = @"\[((\d+,)+\d+\])";
        private string patternMatrix = $@"(\[({patternVector},)+{patternVector}\])|(\[{patternVector}\])";
        //везде в регулярках цепляем первое значение, там их много, но разбираться мне в это в лень

        /// <inheritdoc />
        public string StrParse { get; private set; }

        /// <inheritdoc />
        public IMethod GetResult() => simplexMethod;

        /// <inheritdoc />
        public void ParseStr (string strparse)
        {
            //удаляем пробелы
            strparse = strparse.Replace(oldChar: ' ' , newChar: char.MinValue);
            this.StrParse = strparse;

            //Получаем Z
            string patternZ = @"(^(\d+),\[)|((^\d+\.\d+),\[)";
            Match matchZ = Regex.Match(strparse , patternZ);
            if (matchZ.Success)
                simplexMethod.Z = Convert.ToDouble(matchZ.Value);
            else
                simplexMethod.Z = 0;

            //Получаем С
            //TODO: Распарсить всё остальное
        }
    }

}
