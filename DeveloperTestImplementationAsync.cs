#region Copyright statement
// --------------------------------------------------------------
// Copyright (C) 1999-2016 Exclaimer Ltd. All Rights Reserved.
// No part of this source file may be copied and/or distributed 
// without the express permission of a director of Exclaimer Ltd
// ---------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DeveloperTestInterfaces;

namespace DeveloperTest
{
    public sealed class DeveloperTestImplementationAsync : IDeveloperTestAsync
    {
        public async Task RunQuestionOne(ICharacterReader reader, IOutputResult output, CancellationToken cancellationToken)
        {
            var wordCounter = new Dictionary<string, int>();
            var currentWord = new StringBuilder();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var currentChar = await reader.GetNextCharAsync(cancellationToken);

                    currentWord = new StringBuilder();
                    while (!IsWordEnded(currentChar))
                    {
                        currentWord.Append(currentChar);
                        currentChar = await reader.GetNextCharAsync(cancellationToken);
                    }

                    AddWord(wordCounter, currentWord.ToString());
                }
            }
            catch (EndOfStreamException)
            {
                AddWord(wordCounter, currentWord.ToString());
            }

            ProcessResult(wordCounter, output);
        }

        private async Task CountWords(
            IDictionary<string, int> wordCounter,
            ICharacterReader reader,
            IOutputResult output,
            CancellationToken cancellationToken)
        {
            var currentWord = new StringBuilder();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var currentChar = await reader.GetNextCharAsync(cancellationToken);

                    currentWord = new StringBuilder();
                    while (!IsWordEnded(currentChar))
                    {
                        currentWord.Append(currentChar);
                        currentChar = await reader.GetNextCharAsync(cancellationToken);
                    }

                    AddWord(wordCounter, currentWord.ToString());
                }
            }
            catch (EndOfStreamException)
            {
                AddWord(wordCounter, currentWord.ToString());
            }

            ProcessResult(wordCounter, output);
        }

        public Task RunQuestionTwo(ICharacterReader[] readers, IOutputResult output, CancellationToken cancellationToken)
        {
            /*ConcurrentDictionary<string, int> wordCounter = new ConcurrentDictionary<string, int>();

            Timer times = new Timer(Console.WriteLine, null, 0, 1000);

            List<Task> tasks = new List<Task>();
            foreach (var reader in readers)
            {
                tasks.Add()
            }*/
            throw new Exception();
        }

        private bool IsWordEnded(char c)
        {
            return char.IsWhiteSpace(c) || char.IsPunctuation(c);
        }

        private void AddWord(IDictionary<string, int> wordCounter, string word)
        {
            word = word.ToLower();

            if (word.Trim() == "")
                return;

            if (wordCounter.ContainsKey(word))
                wordCounter[word]++;
            else
                wordCounter.Add(word, 1);
        }

        private void ProcessResult(IDictionary<string, int> wordCounter, IOutputResult output)
        {
            foreach (var word in wordCounter
                .OrderByDescending(w => w.Value)
                .ThenBy(w => w.Key))
            {
                output.AddResult($"{word.Key} - {word.Value}");
            }
        }

        private async Task CountWords(
            IDictionary<string, int> wordCounter,
            ICharacterReader reader,
            IOutputResult output,
            CancellationToken cancellationToken,
            int outputRepeat = 10)
        {
            var currentWord = new StringBuilder();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var currentChar = await reader.GetNextCharAsync(cancellationToken);

                    currentWord = new StringBuilder();
                    while (!IsWordEnded(currentChar))
                    {
                        currentWord.Append(currentChar);
                        currentChar = await reader.GetNextCharAsync(cancellationToken);
                    }

                    AddWord(wordCounter, currentWord.ToString());
                }
            }
            catch (EndOfStreamException)
            {
                AddWord(wordCounter, currentWord.ToString());
            }

            ProcessResult(wordCounter, output);
        }
    }
}