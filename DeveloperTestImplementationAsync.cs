#region Copyright statement
// --------------------------------------------------------------
// Copyright (C) 1999-2016 Exclaimer Ltd. All Rights Reserved.
// No part of this source file may be copied and/or distributed 
// without the express permission of a director of Exclaimer Ltd
// ---------------------------------------------------------------
#endregion
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
        private static readonly object _lock = new object();
        public async Task RunQuestionOne(ICharacterReader reader, IOutputResult output, CancellationToken cancellationToken)
        {
            var wordCounter = new Dictionary<string, int>();

            await CountWordsAsync(wordCounter, reader, output, cancellationToken);
        }

        public async Task RunQuestionTwo(ICharacterReader[] readers, IOutputResult output, CancellationToken cancellationToken)
        {
            Dictionary<string, int> wordCounter = new Dictionary<string, int>();

            // Have never used this thing before and didn't have much time to dive into it.
            //Timer timer = new Timer(ProcessResult, wordCounter, 0, 1000);

            List<Task> tasks = new List<Task>();
            foreach (var reader in readers)
            {
                tasks.Add(CountWordsAsync(wordCounter, reader, output, cancellationToken));
            }

            await Task.WhenAll(tasks);

            ProcessResult(wordCounter, output);
        }

        private async Task CountWordsAsync(
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
                    var prevChar = currentChar;

                    currentWord = new StringBuilder();
                    while (!IsWordEnded(prevChar, currentChar))
                    {
                        currentWord.Append(currentChar);
                        prevChar = currentChar;
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

        private bool IsWordEnded(char prev, char curr)
        {
            return char.IsWhiteSpace(curr)
                || curr == '`'
                || (prev == '-' && prev == curr)
                || (curr != '-' && char.IsPunctuation(curr));
        }

        private void AddWord(IDictionary<string, int> wordCounter, string word)
        {
            word = word.ToLower();

            if (word.Trim() == string.Empty)
                return;

            lock (_lock)
            {
                if (wordCounter.ContainsKey(word))
                    wordCounter[word]++;

                else
                    wordCounter.Add(word, 1);
            }



            /*if (wordCounter.ContainsKey(word))
                wordCounter[word]++;

            else if (wordCounter is ConcurrentDictionary<string, int> concurrentDictionary)
                concurrentDictionary.AddOrUpdate(word, 1, (key, value) => value + 1);

            else
                wordCounter.Add(word, 1);*/
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
    }
}