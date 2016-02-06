﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace CommunityCoreLibrary
{
    public static class Def_Extensions
    {
        /// <summary>
        /// hold a cached list of def -> helpdef links
        /// </summary>
        private static Dictionary<Def, HelpDef> _cachedDefHelpDefLinks = new Dictionary<Def, HelpDef>(); 

        /// <summary>
        /// hold a cached list of icons per def
        /// </summary>
        private static Dictionary<Def, Texture2D> _cachedDefIcons = new Dictionary<Def, Texture2D>(); 

        /// <summary>
        /// Get the helpdef associated with the current def, or null if none exists.
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        public static HelpDef GetHelpDef(this Def def)
        {
            if ( _cachedDefHelpDefLinks.ContainsKey( def ) )
            {
                return _cachedDefHelpDefLinks[def];
            }
            _cachedDefHelpDefLinks.Add( def, DefDatabase<HelpDef>.AllDefsListForReading.FirstOrDefault( hd => hd.keyDef == def ) );
            return _cachedDefHelpDefLinks[def];
        }

        /// <summary>
        /// Get the label, capitalized and given appropriate styling ( bold if def has a helpdef, italic if def has no helpdef but does have description. )
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string LabelStyled( this Def def )
        {
            if ( def.label.NullOrEmpty() )
            {
                return string.Empty;
            }
            if ( def.GetHelpDef() != null )
            {
                return "<b>" + def.LabelCap + "</b>";
            }
            if ( !def.description.NullOrEmpty() )
            {
                return "<i>" + def.LabelCap + "</i>";
            }
            return def.LabelCap;
        }

        /// <summary>
        /// Get the uiIcon for the def. Only defined for buildableDef, and switches to entityToBuild's uiIcon when appropriate.
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        public static Texture2D Icon( this Def def )
        {
            // debug flag
            bool debug = false;

            // check cache
            if ( _cachedDefIcons.ContainsKey( def ) )
            {
                return _cachedDefIcons[def];
            }

            // otherwise try to determine icon
            var bdef = def as BuildableDef;
            var tdef = def as ThingDef;
            var pdef = def as PawnKindDef;

            // animals need special treatment ( this will still only work for animals, pawns are a whole different can o' worms ).
            if( pdef != null )
            {
                if( debug )
                    Log.Message( def.LabelCap + " animal icon " );
                try
                {
                    _cachedDefIcons.Add( def, (pdef.lifeStages.Last().bodyGraphicData.Graphic.MatFront.mainTexture as Texture2D).Crop() );
                    return _cachedDefIcons[def];
                }
                catch
                {
                    if( debug )
                        Log.Message( "failed" );
                }
            }

            // if not buildable it probably doesn't have an icon.
            if ( bdef == null )
            {
                if( debug )
                    Log.Message( def.LabelCap + " not buildable - no icon " );
                _cachedDefIcons.Add( def, null );
                return null;
            }

            // fetch uiIcon. If not set in xml it should be set in Verse.BuildableDef.PostLoad(). 
            if ( tdef?.entityDefToBuild != null )
            {
                if( debug )
                    Log.Message( def.LabelCap + " getting icon from entityToBuild " );
                _cachedDefIcons.Add( def, tdef.entityDefToBuild.Icon().Crop() );
                return tdef.entityDefToBuild.uiIcon;
            }

            if( debug )
                Log.Message( def.LabelCap + " uiIcon " );
            _cachedDefIcons.Add( def, bdef.uiIcon.Crop() );
            return bdef.uiIcon;
        }

        public static float StyledLabelAndIconSize( this Def def )
        {
            var WW = Text.WordWrap;
            Text.WordWrap = false;
            float width = Text.CalcSize( def.LabelStyled() ).x + (def.Icon() == null ? 0 : 20 );
            Text.WordWrap = WW;
            return width;
        }
    }
}
