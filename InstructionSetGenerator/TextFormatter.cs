using System;
using System.Collections.Generic;
using System.Text;

namespace InstructionSetGenerator
{
    class TextFormatter
    {
        public enum IndentChange
        {
            None,
            Increase,
            Decrease
        }

        private StringBuilder stringBuilder = new StringBuilder();
        private string indentString;

        public uint IndentLevel { get; set; } = 0;

        public TextFormatter(string indentString)
        {
            this.indentString = indentString;
        }

        private void InsertIndentation()
        {
            for (int i = 0; i < IndentLevel; i++)
            {
                stringBuilder.Append(indentString);
            }
        }

        public void IncreaseIndentLevel()
        {
            IndentLevel++;
        }

        public bool DecreaseIndentLevel()
        {
            if (IndentLevel > 0)
            {
                IndentLevel--;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AppendLine(string appendLine, IndentChange change)
        {
            switch (change)
            {
                case IndentChange.Increase:
                    IndentLevel++;
                    break;
                case IndentChange.Decrease:
                    if (IndentLevel > 0)
                        IndentLevel--;
                    break;
                default: break;
            }
            InsertIndentation();
            stringBuilder.AppendLine(appendLine);
        }

        public void AppendLine(string appendLine)
        {
            InsertIndentation();
            stringBuilder.AppendLine(appendLine);
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    }
}
