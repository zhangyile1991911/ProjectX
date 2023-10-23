using System.Collections;
using System.Collections.Generic;


class RecipeTrieNode
{
    public int Value { get; }
    public Dictionary<int, RecipeTrieNode> Children { get; }
    public HashSet<int> Recipe { get; }
    public bool IsEndOfWord { get; set; }

    public RecipeTrieNode(int value)
    {
        Value = value;
        Children = new Dictionary<int, RecipeTrieNode>();
        Recipe = new HashSet<int>(5);
        IsEndOfWord = false;
    }
}
public class RecipeTrie
{
    private readonly RecipeTrieNode _root;
    
    public RecipeTrie()
    {
        _root = new RecipeTrieNode(0);
    }

    public void Insert(List<int> items,int recipeId)
    {
        RecipeTrieNode current = _root;

        foreach (int c in items)
        {
            if (!current.Children.ContainsKey(c))
            {
                current.Children[c] = new RecipeTrieNode(c);
            }

            current.Children[c].Recipe.Add(recipeId);
            current = current.Children[c];
        }

        current.IsEndOfWord = true;
    }

    public bool Search(List<int> foods)
    {
        RecipeTrieNode node = FindNode(foods);
        return node != null && node.IsEndOfWord;
    }
    
    private RecipeTrieNode FindNode(List<int> prefix)
    {
        RecipeTrieNode current = _root;

        foreach (char c in prefix)
        {
            if (current.Children.TryGetValue(c, out var child))
            {
                current = child;
            }
            else
            {
                return null;
            }
        }

        return current;
    }

    // public bool StartsWith(List<int> prefix)
    // {
    //     RecipeTrieNode node = FindNode(prefix);
    //     return node != null;
    // }

    public HashSet<int> StartsWith(List<int> food)
    {
        HashSet<int> result = new HashSet<int>(5);
        // if (_root.Children.ContainsKey(food[0]) == false)
        // {
        //     return result;
        // }
        // RecipeTrieNode current = _root.Children[food[0]];
        RecipeTrieNode current = _root;
        foreach (var one in food)
        {
            if (current.Children.TryGetValue(one, out var child))
            {
                current = child;
            }
            else
            {
                return result;
            }
            result.UnionWith(current.Recipe);
        }

        return result;
    }

    
}
