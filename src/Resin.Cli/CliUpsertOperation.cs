﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Resin.Analysis;

namespace Resin.Cli
{
    public class CliUpsertOperation : StreamUpsertOperation
    {
        public CliUpsertOperation(string directory, IAnalyzer analyzer, string jsonFileName, int take = Int32.MaxValue) : base(directory, analyzer, jsonFileName, take)
        {
        }

        public CliUpsertOperation(string directory, IAnalyzer analyzer, Stream jsonFile, int take = Int32.MaxValue) : base(directory, analyzer, jsonFile, take)
        {
        }

        protected override IDictionary<string, string> Parse(string document)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(document);
        }
    }
}