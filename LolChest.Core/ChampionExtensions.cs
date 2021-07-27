using System;
using System.ComponentModel.DataAnnotations;
using Camille.Enums;
using EnumsNET;

namespace LolChest.Core
{
    public static class ChampionExtensions
    {
        public static string GetFriendlyName(this int championId)
        {
            try
            {
                var champion = Champion.NONE;

                if (Enum.IsDefined(typeof(Champion), championId))
                {
                    champion = (Champion)championId;
                }

                return ((DisplayAttribute)champion.GetAttributes().Get(typeof(DisplayAttribute))).Description;
            }
            catch (Exception)
            {
                return " ? ";
            }
        }
    }
}