using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelFace {

    public int normal;
    public float normalValue;
    public int slabNumber;
    public Vector3 origin;

    public SpreadingType spreadingType;
    public FaceDimension dimension;
    public SlabProbabilities slabProbabilities;
    public List<GameObject> createdFace;


    // unique level ?

    public enum SpreadingType{Rect, Neighbor };

    [System.Serializable]
    public class FaceDimension
    {
        public int Xmin;
        public int Xmax;
        public int Ymin;
        public int Ymax;

        public FaceDimension(int xmin, int xmax, int ymin, int ymax)
        {
            Xmin = xmin;
            Xmax = xmax;
            Ymin = ymin;
            Ymax = ymax;
        }

        public void Dimension()
        {
            Xmin = 0;
            Xmax = 0;
            Ymin = 0;
            Ymax = 0;
        }
    }

    [System.Serializable]
    public class SlabProbabilities
    {
        public int permutableProba = 1;
        public int rotativeProba = 1;
        public int staticProba = 0;
    }

    public string GetRandomSlabType()
    {
        Dictionary<string, int> weights = new Dictionary<string, int>()
        {
            {"permutable",slabProbabilities.permutableProba},
            {"rotative",slabProbabilities.rotativeProba},
            {"static",slabProbabilities.staticProba}
        };

        string selectedType = WeightedRandomizer.From(weights).TakeOne();

        return selectedType;
    }

    //public string GetRandomSlabType()
    //{
    //    LevelFace.SlabProbabilities probabilities = slabProbabilities;

    //    float probaSum = probabilities.permutableProba + probabilities.permutableProba + probabilities.staticProba;
    //    float randomValue = Random.Range(0f, probaSum);

    //    if(randomValue < probabilities.rotativeProba)
    //    {
    //        return "permutable";
    //    }
    //    else if (randomValue < probabilities.permutableProba + probabilities.rotativeProba)
    //    {
    //        return "rotative";
    //    }
    //    else
    //    {
    //        return "static";
    //    }

    //}


}
