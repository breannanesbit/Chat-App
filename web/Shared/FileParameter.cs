using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
        public class FileParameter
        {
            public byte[] Data { get; }
            public string FileName { get; }

            public FileParameter(byte[] data, string fileName)
            {
                Data = data ?? throw new ArgumentNullException(nameof(data));
                FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            }
        }

}
