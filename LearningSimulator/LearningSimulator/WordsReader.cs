using LearningSimulator.Models;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using ProgressBar = Xamarin.Forms.ProgressBar;

namespace LearningSimulator
{
    public class WordsReader
    {
        /// <summary>
        /// Считывает слова по умолчанию из ресурса words.txt 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> ReadWords(User user)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resources = assembly.GetManifestResourceNames();
            foreach (string resource in resources)
            {
                if (resource.EndsWith(".txt"))
                {
                    if (resource.ToLowerInvariant() == "learningsimulator.words.txt")
                    {
                        Stream stream = assembly.GetManifestResourceStream(resource);
                        if (stream != null)
                        {
                            string text = "";
                            using (var reader = new StreamReader(stream))
                            {
                                text = reader.ReadToEnd();
                                string[] fileLines = text.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                string[] temp;
                                string meaning = string.Empty, translation = string.Empty;
                                byte partOfSpeech = 0;
                                for (int i = 0; i < fileLines.Length; i++)
                                {
                                    temp = fileLines[i].Split(new string[2] { " - ", " [" }, StringSplitOptions.RemoveEmptyEntries);
                                    for (int j = 0; j < temp.Length; j++)
                                    {
                                        temp[j] = temp[j].Trim();
                                        switch (j)
                                        {
                                            case 0:
                                                meaning = temp[j];
                                                break;
                                            case 1:
                                                translation = temp[j];
                                                break;
                                            case 2:
                                                partOfSpeech = Convert.ToByte(temp[j].Substring(0, 1));
                                                break;
                                        }
                                    }
                                    await App.Database.SaveWordAsync(user, new Word() { Meaning = meaning, PartOfSpeech = partOfSpeech, Translation = translation });
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public async Task<bool> ReadVerbs(User user)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resources = assembly.GetManifestResourceNames();
            foreach (string resource in resources)
            {
                if (resource.EndsWith(".txt"))
                {
                    if (resource.ToLowerInvariant() == "learningsimulator.phrasal_verbs.txt")
                    {
                        Stream stream = assembly.GetManifestResourceStream(resource);
                        if (stream != null)
                        {
                            string text = "";
                            using (var reader = new StreamReader(stream))
                            {
                                text = reader.ReadToEnd();
                                string[] fileLines = text.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                string[] temp;
                                string meaning = string.Empty, translation = string.Empty;
                                for (int i = 0; i < fileLines.Length; i++)
                                {
                                    temp = fileLines[i].Split(new string[1] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                                    for (int j = 0; j < temp.Length; j++)
                                    {
                                        temp[j] = temp[j].Trim();
                                        switch (j)
                                        {
                                            case 0:
                                                meaning = temp[j];
                                                break;
                                            case 1:
                                                translation = temp[j];
                                                break;
                                        }
                                    }
                                    await App.Database.SavePVAsync(user, new PhrasalVerb() { Meaning = meaning, Translation = translation });
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

            /// <summary>
            /// Конвертирует ответ юзера на контрольный вопрос, если это дата
            /// </summary>
            /// <param name="answer"></param>
            /// <returns></returns>
            public string ConvertAnswer(string answer)
        {
            string[] parts = answer.Split(new char[3] { '/', '.', '-' }, StringSplitOptions.RemoveEmptyEntries);
            string res = string.Empty;
            parts.ForEach(part => { res += part; }) ; 
            return res;
        }
    }
}
