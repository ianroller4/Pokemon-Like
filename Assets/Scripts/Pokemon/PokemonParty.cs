using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PokemonParty : MonoBehaviour {

    [SerializeField] List<Pokemon> pokemon;

    public List<Pokemon> Pokemon { get => pokemon; set => pokemon = value; }

    private void Start () {

        foreach (var p in Pokemon) {
            p.Init();
        }

    }

    public Pokemon GetHealthyPokemon () {

        return Pokemon.Where(x => x.HP > 0).FirstOrDefault();

    } 
    
}
