using System;
using System.Collections.Generic;
using System.Text;

namespace TCG.Card.Subclasses
{
    
    public enum CardElements
    {
        Fire = 1,       //fire   > normal, darkness, arcane 
        Water,      //wasser > fire, fel
        Normal,     //normal > water, arcane
        Darkness,   //darkness > normal, arcane
        Light,      //light > darkness,
        Arcane,     //arcane > 
        Fel         //fel > normal, light, 
    }
    public enum CardTypes
    {
        Spell = 1,
        Monster
    }
}
