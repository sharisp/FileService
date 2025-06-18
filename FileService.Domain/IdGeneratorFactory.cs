using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IdGen;
using Microsoft.Extensions.Configuration;

namespace FileService.Domain
{
    public static class IdGeneratorFactory
    {
        private static IdGenerator _generator;

        public static void Initialize(int workerId)
        {
            _generator = new IdGenerator(workerId);
        }

        // private static readonly IdGenerator _generator = new IdGenerator(0);

        public static long NewId() => _generator.CreateId();
    }
}
