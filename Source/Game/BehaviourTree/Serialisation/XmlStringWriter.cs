using System.IO;
using System.Text;

namespace BehaviourTree
{
    public class XmlStringWriter : StringWriter
    {
        public XmlStringWriter(StringBuilder sb, Encoding encoding)
            : base(sb)
        {
            this.m_Encoding = encoding;
        }
        private readonly Encoding m_Encoding;
        public override Encoding Encoding
        {
            get
            {
                return this.m_Encoding;
            }
        }
    }
}
