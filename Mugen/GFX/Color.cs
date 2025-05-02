using Microsoft.Xna.Framework;
using Mugen.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mugen.GFX
{
    public struct ColorI
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color ToColor()
        {
            return new Color(R, G, B, A);
        }

        public override string ToString()
        {
            return "{ R=" + R + ", G=" + G + ", B=" + B + ", A=" + A + " }";
        }
    }
    public struct ColorF
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color ToColor()
        {
            return new Color(R, G, B, A);
        }

        public override string ToString()
        {
            return "{ R=" + R + ", G=" + G + ", B=" + B + ", A=" + A + " }";
        }

    }

    public static class HSV
    {
        public static Color ToRadRGB(float h = Geo.RAD_360, float s = 1.0f, float v = 1.0f)
        {
            return ToRGB((float)Geo.RadToDeg(h), s, v);
        }
        public static Color ToRGB(float h = 360f, float s = 1.0f, float v = 1.0f)
        {
            // Normalisation des entrées
            h = h % 360f; // Assure que H reste entre 0 et 360
            s = Math.Clamp(s, 0f, 1f); // S entre 0 et 1
            v = Math.Clamp(v, 0f, 1f); // V entre 0 et 1

            float r = 0, g = 0, b = 0;

            if (s == 0) // Cas où la couleur est grise (saturation = 0)
            {
                r = g = b = v;
            }
            else
            {
                // Calcul des secteurs et des valeurs intermédiaires
                int sector = (int)(h / 60f); // Divise le cercle en 6 secteurs (0 à 5)
                float fraction = (h / 60f) - sector; // Partie décimale dans le secteur
                float p = v * (1 - s);
                float q = v * (1 - s * fraction);
                float t = v * (1 - s * (1 - fraction));

                // Attribution des valeurs RGB selon le secteur
                switch (sector)
                {
                    case 0: // Rouge → Jaune
                        r = v; g = t; b = p; break;
                    case 1: // Jaune → Vert
                        r = q; g = v; b = p; break;
                    case 2: // Vert → Cyan
                        r = p; g = v; b = t; break;
                    case 3: // Cyan → Bleu
                        r = p; g = q; b = v; break;
                    case 4: // Bleu → Magenta
                        r = t; g = p; b = v; break;
                    case 5: // Magenta → Rouge
                        r = v; g = p; b = q; break;
                }
            }

            // Conversion en valeurs RGB (0-255)
            return new Color(
                (byte)(r * 255f),
                (byte)(g * 255f),
                (byte)(b * 255f)
            );
        }
        /// <summary>
        /// Modifie une couleur en appliquant des ajustements HSV.
        /// </summary>
        /// <param name="sourceColor">Couleur d'origine (RGB).</param>
        /// <param name="hueShift">Décalage de la teinte (0-360, peut être null pour ignorer).</param>
        /// <param name="saturationMultiplier">Multiplicateur de saturation (ex. 1.0f = inchangé, peut être null).</param>
        /// <param name="valueMultiplier">Multiplicateur de valeur (ex. 1.0f = inchangé, peut être null).</param>
        /// <returns>Couleur résultante (RGB).</returns>
        public static Color Adjust(this Color sourceColor, float? hueShift = null, float? saturationMultiplier = null, float? valueMultiplier = null)
        {
            // Convertir RGB en HSV
            (float h, float s, float v) = RgbToHsv(sourceColor);

            // Appliquer les ajustements
            if (hueShift.HasValue)
            {
                h = (h + hueShift.Value) % 360f; // Garder la teinte dans [0, 360]
                if (h < 0) h += 360f;
            }

            if (saturationMultiplier.HasValue)
            {
                s = MathHelper.Clamp(s * saturationMultiplier.Value, 0f, 1f);
            }

            if (valueMultiplier.HasValue)
            {
                v = MathHelper.Clamp(v * valueMultiplier.Value, 0f, 1f);
            }

            // Convertir HSV en RGB
            return ToRGB(h, s, v);
        }

        /// <summary>
        /// Convertit une couleur RGB en HSV.
        /// </summary>
        /// <param name="color">Couleur RGB.</param>
        /// <returns>(Hue [0-360], Saturation [0-1], Value [0-1]).</returns>
        private static (float h, float s, float v) RgbToHsv(Color color)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            float h = 0f;
            float s = 0f;
            float v = max;

            // Calculer la saturation
            if (max > 0f)
            {
                s = delta / max;
            }

            // Calculer la teinte
            if (delta > 0f)
            {
                if (r == max)
                    h = (g - b) / delta; // Entre jaune et magenta
                else if (g == max)
                    h = 2f + (b - r) / delta; // Entre cyan et jaune
                else
                    h = 4f + (r - g) / delta; // Entre magenta et cyan

                h *= 60f; // Convertir en degrés
                if (h < 0f) h += 360f;
            }
            else
            {
                h = 0f; // Teinte indéfinie (gris)
            }

            return (h, s, v);
        }

        /// <summary>
        /// Convertit une couleur HSV en RGB.
        /// </summary>
        /// <param name="h">Teinte [0-360].</param>
        /// <param name="s">Saturation [0-1].</param>
        /// <param name="v">Valeur [0-1].</param>
        /// <returns>Couleur RGB.</returns>
        //private static Color HsvToRgb(float h, float s, float v)
        //{
        //    // Cas sans saturation (gris)
        //    if (s <= 0f)
        //    {
        //        int gray = (int)MathHelper.Clamp(v * 255f, 0f, 255f);
        //        return new Color(gray, gray, gray);
        //    }

        //    // Normaliser la teinte
        //    h = h % 360f;
        //    if (h < 0f) h += 360f;

        //    // Calcul des composantes RGB
        //    float hSector = h / 60f; // Secteur de 0 à 5
        //    int i = (int)hSector;
        //    float f = hSector - i; // Partie fractionnaire

        //    float p = v * (1f - s);
        //    float q = v * (1f - s * f);
        //    float t = v * (1f - s * (1f - f));

        //    float r, g, b;
        //    switch (i)
        //    {
        //        case 0: r = v; g = t; b = p; break;
        //        case 1: r = q; g = v; b = p; break;
        //        case 2: r = p; g = v; b = t; break;
        //        case 3: r = p; g = q; b = v; break;
        //        case 4: r = t; g = p; b = v; break;
        //        default: r = v; g = p; b = q; break;
        //    }

        //    // Convertir en valeurs RGB (0-255) et retourner
        //    return new Color(
        //        (int)MathHelper.Clamp(r * 255f, 0f, 255f),
        //        (int)MathHelper.Clamp(g * 255f, 0f, 255f),
        //        (int)MathHelper.Clamp(b * 255f, 0f, 255f));
        //}

    }
}
