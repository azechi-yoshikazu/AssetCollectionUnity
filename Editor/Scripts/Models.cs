using System;
using System.Collections.Generic;

namespace AssetCollection.Models
{
    [Serializable]
    internal class CollectionList
    {
        public List<Collection> Collections = new List<Collection>();

        public Collection AddNewCollection(string name = null)
        {
            name = name ?? "NewCollection";

            try
            {
                var newCollection = new Collection(AppendNumber(name));
                Collections.Add(newCollection);
                return newCollection;
            }
            catch (Exception _)
            {
                return null;
            }

        }

        public Collection FindBy(string name) => Collections.Find((x) => x.Name == name);
        public Collection FindBy(int index) => Collections.Count > index ? Collections[index] : null;
        public bool Contains(string name) => FindBy(name) != null;

        private string AppendNumber(string name)
        {
            if (!Contains(name))
            {
                return name;
            }
            const int maxIteration = 100;

            for (int number = 1; number < maxIteration; number++)
            {
                string modifiedName = $"{name}_{number}";

                if (!Contains(modifiedName))
                {
                    return modifiedName;
                }
            }

            return null;
        }
    }

    [Serializable]
    internal class Collection
    {
        public string Name;
        public List<string> Assets = new List<string>();

        public Collection(string name = null) => Name = name ?? "NewCollection";
    }
};
