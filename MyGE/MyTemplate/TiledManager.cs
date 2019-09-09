using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;

namespace MyGE
{
    /* Classe utilitaire de gestion des map crées avec Tiled V : 1.1.4
     * M. Le Thiec
     * création : 31/03/2018
     * V 1.00
     * 
     * ATTENTION : pour utiliser TiledSharp, il faut ajouter la Référence System.Xml.Linq
     * 
     */

    public class TiledManager
    {
        private MainGame mainGame;
        private TmxMap map;
        private Texture2D tileSetTexture;
        private int mapWidth; // en lignes et colonnes
        private int mapHeight;
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        private List<Rectangle> tileCollisions; // liste de rectangles pour gérer les collisions
        private List<List<Rectangle>> objLayers;
        private string orientation; // type de map entre [unknown, orthogonal, isometric, straggered, hexagonal]
        private int tileSetLenght; // dimension en ligne et colonnes de la tileset
        public Point OverlapingPos;

        // --------------------- PROPERTIES -------------------
        public List<Rectangle> Collides
        {
            get { return tileCollisions; }
            private set { tileCollisions = value; }
        }

        //---------------------- CONSTRUCTEUR ----------------

        public TiledManager(MainGame pGame)
        {
            mainGame = pGame;
            objLayers = new List<List<Rectangle>>();
            tileCollisions = new List<Rectangle>();
        }

        // --------------------- FONCTIONS -------------------
        public void loadMap(string pMapName)
        {
            // charge la nouvelle map
            map = new TmxMap(pMapName);
            // charge la texture du tileSet -->> devra migrer vers la classe AssetManager
            tileSetTexture = AssetManager.LoadImage(map.Tilesets[0].Name);

            // Initialisation des variables de la map
            TileWidth = map.TileWidth;
            TileHeight = map.TileHeight;
            mapWidth = map.Width;
            mapHeight = map.Height;
            orientation = map.Orientation.ToString();
            tileSetLenght = (int)map.Tilesets[0].Columns;

            // initialisation des rectangles de collisions
            for (int i = 0; i < map.ObjectGroups.Count; i++)
            {
                objLayers.Add(new List<Rectangle>());
                LayerRectangles(i);
            }
            // debug
            Console.WriteLine("collide layer : " + map.Layers[1].Properties.ContainsKey("CollideLayer"));
            Console.WriteLine("tile id : " + GetTileProperty(0, "solid"));
            Console.WriteLine("test de recherche de property dans une tile : " + map.Tilesets[0].Tiles.Keys.Contains(8));
        }

        public int GetTileIdAt(string pLayerName, Point mapCoordinates)
        // renvoie l'id d'une tuile à la coordonnée donnée dans le point sous la forme colonne, ligne
        {
            // récupère le layer concerné
            int layer = GetIndexLayerByName(pLayerName);
            // calcule la position de la tile en faisant : ((nbr de ligne - 1) x largeur map + position colonne)
            int idPosition = (mapCoordinates.Y * mapWidth + mapCoordinates.X);
            int GID = -1; // -1 equivaut une tuile vide, donc non prise en compte
            // on ne prends en compte que les cas de figure qui sont dans le cadre de la map
            if (mapCoordinates.X >= 0 && mapCoordinates.X < map.Width &&
                mapCoordinates.Y >= 0 && mapCoordinates.Y < map.Height)
            {
                GID = (map.Layers[layer].Tiles[idPosition].Gid) -1; // -1 car les ID de tuiles commencent à 0
            }
            return GID;
        }

        public string GetTileProperty(int pTileID, string pKey)
            // renvoie la valeur de la clé <key> pour la tuile N° pTileNumber si elle existe
        {
            string returnVal = null;
            // On vérifie que l'indice de tuile demandé existe avec une clé
            if (map.Tilesets[0].Tiles.Keys.Contains(pTileID))
            {
                // On vérifie que la clé demandée existe dans la map
                if (map.Tilesets[0].Tiles[pTileID].Properties.ContainsKey(pKey))
                {
                    returnVal = map.Tilesets[0].Tiles[pTileID].Properties[pKey];
                }
            }
           
            return returnVal;
        }

        private int GetIndexLayerByName(string pLayerName)
            // recherche l'index du layer à partir du nom fournis
        {
            var id = 0;
            for (int i = 0; i < map.Layers.Count; i++)
            {
                if (map.Layers[i].Name == pLayerName)
                    id = i;
            }
            return id;
        }

        public void InitCollideLayer(string pLayerName)
            // Construit une liste de Rectangle pour chaque tuile présente sur le calque fourni par pLayerName
            // Cette liste de rectangle servira ensuite pour les controles de collisions
        {
            var index = GetIndexLayerByName(pLayerName);
            for (int i = 0; i < map.Layers[index].Tiles.Count; i++)
            {
                if (map.Layers[index].Tiles[i].Gid != 0)
                {
                    // création d'un rectangle temporaire. Les coordonnées sont * la taille des tuiles pour
                    // obtenir directement un rectangle à la bonne position écran
                    var rect = new Rectangle(map.Layers[index].Tiles[i].X * TileWidth,
                                              map.Layers[index].Tiles[i].Y * TileHeight,
                                              TileWidth, TileHeight);
                    tileCollisions.Add(rect);
                }
            }
        }

        private void LayerRectangles(int pLayerNum)
            // extrait les rectangles du layer passé en argument. Le layer doit être de type ObjectGroups
        {
            var layer = map.ObjectGroups[pLayerNum]; // récupère le layer d'objets
            //Console.WriteLine("index du layer object : " + pLayerNum + "\tName : " + layer.Name);
            for (int i = 0; i < layer.Objects.Count; i++)
            {
                var x = (int)layer.Objects[i].X;
                var y = (int)layer.Objects[i].Y;
                var w = (int)layer.Objects[i].Width;
                var h = (int)layer.Objects[i].Height;
                var rect = new Rectangle(x, y, w, h);
                objLayers[pLayerNum].Add(rect);
            }
            /*
            Console.WriteLine("controle de resultats ...");
            foreach (Rectangle item in objLayers[pLayerNum])
            {
                Console.WriteLine("rect : " + item.X + " , " + item.Y + " , " + item.Width + " , " + item.Height);
            }*/
        }

        public Point GetMapCoordinates(Point pScreenPosition)
            // calcule et renvoie les coordonnées convertie en ligne et colonne de map
        {
            Point mapCoordinates = new Point()
            {
                X = (int)Math.Floor((double)pScreenPosition.X / TileWidth),
                Y = (int)Math.Floor((double)pScreenPosition.Y / TileHeight)
            };
            return mapCoordinates;
        }

        public Vector2 Convert2MapCoords(Point pScreenPosition)
        {
            Point mapCoordinates = GetMapCoordinates(pScreenPosition);
            return new Vector2(mapCoordinates.X * TileWidth, mapCoordinates.Y * TileHeight);
        }

        public bool IsOverlaping(Vector2 pCoordinates)
            // test si une position est en collision avec un rectangle de collision
        {
            foreach (Rectangle item in tileCollisions)
            {
                if (item.Contains(pCoordinates))
                    return true;
            }
            return false;
        }

        public bool IsOverlaping(Point pCoordinates)
        // test si une position est en collision avec un rectangle de collision
        // Point = coordonnées souris
        {
            foreach (Rectangle item in tileCollisions)
            {
                if (item.Contains(pCoordinates))
                    return true;
            }
            return false;
        }

        public bool IsOverlaping(Rectangle pRect)
            // test si un rectangle intersecte une des tuiles
        {
            foreach (Rectangle item in tileCollisions)
            {
                if (item.Intersects(pRect))
                {
                    OverlapingCoordinate(item, pRect);
                    return true;
                }
            }
            return false;
        }

        private void OverlapingCoordinate(Rectangle item, Rectangle pRect)
        {
            if (pRect.Bottom > item.Top)
                OverlapingPos.Y = item.Top;
        }

        private void DestroyTile(Point pTilePosition, string pLayer)
            // supprime une tuile de la map en mémoire
            // supprime également le rectangle de collision correspondant
            // nécessite de parcourir tous les layers
        {
            var layerID = GetIndexLayerByName(pLayer);

        }


        public void Update(GameTime gameTime)
            // cas des tuiles animées
        {

        }

        public void Draw(GameTime gameTime)
        {
            mainGame.spriteBatch.Begin();
            // dessin de la map

            for (int layer = 0; layer < map.Layers.Count; layer++)
            {
                //Console.WriteLine("draw layer " + map.Layers[layer].Name);
                for (int tile = 0; tile < map.Layers[layer].Tiles.Count; tile++)
                {
                    // récupère l'identifiant de la tuile en cours de lecture dans la map
                    var GID = map.Layers[layer].Tiles[tile].Gid;
                    if (GID != 0) // le GID à 0 est toujours un vide, on ne l'affiche pas !
                    {
                        var X = map.Layers[layer].Tiles[tile].X;
                        var Y = map.Layers[layer].Tiles[tile].Y;
                        var tileSetLine = ((int)Math.Floor((double)((GID - 1) / tileSetLenght)));
                        var tileSetCol = (GID - 1) % tileSetLenght; // calcule la position en colonne dans le tileset

                        mainGame.spriteBatch.Draw(tileSetTexture, new Vector2(X * TileWidth, Y * TileHeight),
                                        new Rectangle(tileSetCol * TileWidth, tileSetLine * TileHeight, TileWidth, TileHeight),
                                        Color.White);
                    }
                   
                }

            }
            mainGame.spriteBatch.End();
        }

    }
}
