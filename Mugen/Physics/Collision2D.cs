using Microsoft.Xna.Framework;
using Mugen.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mugen.Physics
{
    public static class Collision2D
    {
        // All Collision Form !
        public static Vector2 GetReboundVec2D(Vector2 v, Vector2 N) // return Vec2 vector of Rebound Vector v & Normal N 
        {
            Vector2 v2;
            float pscal = (v.X * N.X + v.Y * N.Y); // Produit scalaire
            v2.X = v.X - 2 * pscal * N.X;
            v2.Y = v.Y - 2 * pscal * N.Y;

            return v2;

        }
        public static float Dist(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
        // POINT/POINT
        public static bool PointPoint(float x1, float y1, float x2, float y2)
        {

            // are the two points in the same location?
            if (x1 == x2 && y1 == y2)
            {
                return true;
            }
            return false;
        }
        // POINT/CIRCLE
        public static bool PointCircle(float px, float py, float cx, float cy, float r)
        {

            // get distance between the point and circle's center
            // using the Pythagorean Theorem
            float distX = px - cx;
            float distY = py - cy;
            float distance = (float)Math.Sqrt((distX * distX) + (distY * distY));

            // if the distance is less than the circle's
            // radius the point is inside!
            if (distance <= r)
            {
                return true;
            }
            return false;
        }
        // CIRCLE/CIRCLE
        public static bool CircleCircle(float c1x, float c1y, float c1r, float c2x, float c2y, float c2r)
        {

            // get distance between the circle's centers
            // use the Pythagorean Theorem to compute the distance
            float distX = c1x - c2x;
            float distY = c1y - c2y;
            float distance = (float)Math.Sqrt((distX * distX) + (distY * distY));

            // if the distance is less than the sum of the circle's
            // radii, the circles are touching!
            if (distance <= c1r + c2r)
            {
                return true;
            }
            return false;
        }
        // POINT/RECTANGLE
        public static bool PointRect(float px, float py, float rx, float ry, float rw, float rh)
        {

            // is the point inside the rectangle's bounds?
            if (px >= rx &&        // right of the left edge AND
                px <= rx + rw &&   // left of the right edge AND
                py >= ry &&        // below the top AND
                py <= ry + rh)
            {   // above the bottom
                return true;
            }
            return false;
        }
        // RECTANGLE/RECTANGLE
        public static bool RectRect(float r1x, float r1y, float r1w, float r1h, float r2x, float r2y, float r2w, float r2h)
        {

            // are the sides of one rectangle touching the other?

            if (r1x + r1w >= r2x &&    // r1 right edge past r2 left
                r1x <= r2x + r2w &&    // r1 left edge past r2 right
                r1y + r1h >= r2y &&    // r1 top edge past r2 bottom
                r1y <= r2y + r2h)
            {    // r1 bottom edge past r2 top
                return true;
            }
            return false;
        }
        // CIRCLE/RECTANGLE
        public static bool CircleRect(float cx, float cy, float radius, float rx, float ry, float rw, float rh)
        {

            // temporary variables to set edges for testing
            float testX = cx;
            float testY = cy;

            // which edge is closest?
            if (cx < rx) testX = rx;      // test left edge
            else if (cx > rx + rw) testX = rx + rw;   // right edge
            if (cy < ry) testY = ry;      // top edge
            else if (cy > ry + rh) testY = ry + rh;   // bottom edge

            // get distance from closest edges
            float distX = cx - testX;
            float distY = cy - testY;
            float distance = (float)Math.Sqrt((distX * distX) + (distY * distY));

            // if the distance is less than the radius, collision!
            if (distance <= radius)
            {
                return true;
            }
            return false;
        }
        // SEGMENT/POINT
        public static bool SegmentPoint(float x1, float y1, float x2, float y2, float px, float py)
        {

            // get distance from the point to the two ends of the line
            float d1 = Dist(px, py, x1, y1);
            float d2 = Dist(px, py, x2, y2);

            // get the length of the line
            float lineLen = Dist(x1, y1, x2, y2);

            // since floats are so minutely accurate, add
            // a little buffer zone that will give collision
            float buffer = 0.1f;    // higher # = less accurate

            // if the two distances are equal to the line's 
            // length, the point is on the line!
            // note we use the buffer here to give a range, 
            // rather than one #
            if (d1 + d2 >= lineLen - buffer && d1 + d2 <= lineLen + buffer)
            {
                return true;
            }
            return false;
        }
        // SEGMENT/CIRCLE
        public static bool SegmentCircle(float x1, float y1, float x2, float y2, float cx, float cy, float r)
        {

            // is either end INSIDE the circle?
            // if so, return true immediately
            bool inside1 = PointCircle(x1, y1, cx, cy, r);
            bool inside2 = PointCircle(x2, y2, cx, cy, r);
            if (inside1 || inside2) return true;

            // get length of the line
            float distX = x1 - x2;
            float distY = y1 - y2;
            float len = (float)Math.Sqrt((distX * distX) + (distY * distY));

            // get dot product of the line and circle
            float dot = (((cx - x1) * (x2 - x1)) + ((cy - y1) * (y2 - y1))) / (float)Math.Pow(len, 2);

            // find the closest point on the line
            float closestX = x1 + (dot * (x2 - x1));
            float closestY = y1 + (dot * (y2 - y1));

            // is this point actually on the line segment?
            // if so keep going, but if not, return false
            bool onSegment = SegmentPoint(x1, y1, x2, y2, closestX, closestY);
            if (!onSegment) return false;

            // optionally, draw a circle at the closest
            // point on the line
            //fill(255, 0, 0);
            //noStroke();
            //ellipse(closestX, closestY, 20, 20);

            // get distance to closest point
            distX = closestX - cx;
            distY = closestY - cy;
            float distance = (float)Math.Sqrt((distX * distX) + (distY * distY));

            if (distance <= r)
            {
                return true;
            }
            return false;
        }
        // SEGMENT/SEGMENT : return intersection
        public static bool SegmentSegment(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {

            // calculate the distance to intersection point
            float uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
            float uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            // if uA and uB are between 0-1, lines are colliding
            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {

                // optionally, draw a circle where the lines meet
                float intersectionX = x1 + (uA * (x2 - x1));
                float intersectionY = y1 + (uA * (y2 - y1));

                //fill(255, 0, 0);
                //noStroke();
                //ellipse(intersectionX, intersectionY, 20, 20);

                return true;
            }
            return false;
        }
        public static bool SegmentSegment(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, out Vector2 intersection)
        {

            // calculate the distance to intersection point
            float uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
            float uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            // if uA and uB are between 0-1, lines are colliding
            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {

                // optionally, draw a circle where the lines meet
                float intersectionX = x1 + (uA * (x2 - x1));
                float intersectionY = y1 + (uA * (y2 - y1));

                //fill(255, 0, 0);
                //noStroke();
                //ellipse(intersectionX, intersectionY, 20, 20);

                intersection = new Vector2(intersectionX, intersectionY);
                return true;
            }
            intersection = default;
            return false;
        }
        // SEGMENT/RECTANGLE
        public static bool SegmentRect(float x1, float y1, float x2, float y2, float rx, float ry, float rw, float rh)
        {

            // check if the line has hit any of the rectangle's sides
            // uses the Line/Line function below
            bool left = SegmentSegment(x1, y1, x2, y2, rx, ry, rx, ry + rh);
            bool right = SegmentSegment(x1, y1, x2, y2, rx + rw, ry, rx + rw, ry + rh);
            bool top = SegmentSegment(x1, y1, x2, y2, rx, ry, rx + rw, ry);
            bool bottom = SegmentSegment(x1, y1, x2, y2, rx, ry + rh, rx + rw, ry + rh);

            // if ANY of the above are true, the line
            // has hit the rectangle
            if (left || right || top || bottom)
            {
                return true;
            }
            return false;
        }
        public static bool SegmentRect(float x1, float y1, float x2, float y2, float rx, float ry, float rw, float rh,
            out Vector2 intersectionLeft,
            out Vector2 intersectionRight,
            out Vector2 intersectionTop,
            out Vector2 intersectionBottom
            )
        {

            // check if the line has hit any of the rectangle's sides
            // uses the Line/Line function below
            bool left = SegmentSegment(x1, y1, x2, y2, rx, ry, rx, ry + rh, out intersectionLeft);
            bool right = SegmentSegment(x1, y1, x2, y2, rx + rw, ry, rx + rw, ry + rh, out intersectionRight);
            bool top = SegmentSegment(x1, y1, x2, y2, rx, ry, rx + rw, ry, out intersectionTop);
            bool bottom = SegmentSegment(x1, y1, x2, y2, rx, ry + rh, rx + rw, ry + rh, out intersectionBottom);

            // if ANY of the above are true, the line
            // has hit the rectangle
            if (left || right || top || bottom)
            {
                return true;
            }
            return false;
        }
        // POLYGON/POINT
        // used only to check if the second polygon is
        // INSIDE the first
        public static bool PolygonPoint(Vector2[] vertices, float px, float py)
        {
            bool collision = false;

            // go through each of the vertices, plus the next
            // vertex in the list
            int next = 0;
            for (int current = 0; current < vertices.Length; current++)
            {

                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == vertices.Length) next = 0;

                // get the PVectors at our current position
                // this makes our if statement a little cleaner
                Vector2 vc = vertices[current];    // c for "current"
                Vector2 vn = vertices[next];       // n for "next"

                // compare position, flip 'collision' variable
                // back and forth
                if (((vc.Y > py && vn.Y < py) || (vc.Y < py && vn.Y > py)) &&
                     (px < (vn.X - vc.X) * (py - vc.Y) / (vn.Y - vc.Y) + vc.X))
                {
                    collision = !collision;
                }
            }
            return collision;
        }
        // POLYGON/LINE
        public static bool PolygonLine(Vector2[] vertices, Vector2 A, Vector2 B)
        {
            // go through each of the vertices, plus the next
            // vertex in the list
            int next = 0;
            for (int current = 0; current < vertices.Length; current++)
            {
                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == vertices.Length) next = 0;

                // get the PVectors at our current position
                // extract X/Y coordinates from each
                Vector2 C = vertices[current];
                Vector2 D = vertices[next];

                // do a Line/Line comparison
                // if true, return 'true' immediately and
                // stop testing (faster)
                bool hit = LineLine(A, B, C, D);
                if (hit)
                {
                    return true;
                }
            }

            // never got a hit
            return false;
        }
        // POLYGON/CIRCLE
        public static bool PolygonCircle(Vector2[] vertices, float cx, float cy, float r)
        {

            // go through each of the vertices, plus
            // the next vertex in the list
            int next = 0;
            for (int current = 0; current < vertices.Length; current++)
            {

                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == vertices.Length) next = 0;

                // get the PVectors at our current position
                // this makes our if statement a little cleaner
                Vector2 vc = vertices[current];    // c for "current"
                Vector2 vn = vertices[next];       // n for "next"

                // check for collision between the circle and
                // a line formed between the two vertices
                bool collision = SegmentCircle(vc.X, vc.Y, vn.X, vn.Y, cx, cy, r);
                if (collision) return true;
            }

            // the above algorithm only checks if the circle
            // is touching the edges of the polygon – in most
            // cases this is enough, but you can un-comment the
            // following code to also test if the center of the
            // circle is inside the polygon

            // boolean centerInside = polygonPoint(vertices, cx,cy);
            // if (centerInside) return true;

            // otherwise, after all that, return false
            return false;
        }
        // POLYGON/RECTANGLE
        public static bool PolygonRect(Vector2[] vertices, float rx, float ry, float rw, float rh)
        {

            // go through each of the vertices, plus the next
            // vertex in the list
            int next = 0;
            for (int current = 0; current < vertices.Length; current++)
            {

                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == vertices.Length) next = 0;

                // get the PVectors at our current position
                // this makes our if statement a little cleaner
                Vector2 vc = vertices[current];    // c for "current"
                Vector2 vn = vertices[next];       // n for "next"

                // check against all four sides of the rectangle
                bool collision = SegmentRect(vc.X, vc.Y, vn.X, vn.Y, rx, ry, rw, rh);
                if (collision) return true;

                // optional: test if the rectangle is INSIDE the polygon
                // note that this iterates all sides of the polygon
                // again, so only use this if you need to
                bool inside = PolygonPoint(vertices, rx, ry);
                if (inside) return true;
            }

            return false;
        }
        // LINE/LINE
        public static bool LineLine(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
        {

            // calculate the direction of the lines
            float uA = ((D.X - C.X) * (A.Y - C.Y) - (D.Y - C.Y) * (A.X - C.X)) / ((D.Y - C.Y) * (B.X - A.X) - (D.X - C.X) * (B.Y - A.Y));
            float uB = ((B.X - A.X) * (A.Y - C.Y) - (B.Y - A.Y) * (A.X - C.X)) / ((D.Y - C.Y) * (B.X - A.X) - (D.X - C.X) * (B.Y - A.Y));

            // if uA and uB are between 0-1, lines are colliding
            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                return true;
            }
            return false;
        }
        // POLYGON/POLYGON
        public static bool PolygonPolygon(Vector2[] p1, Vector2[] p2)
        {

            // go through each of the vertices, plus the next
            // vertex in the list
            int next = 0;
            for (int current = 0; current < p1.Length; current++)
            {

                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == p1.Length) next = 0;

                // get the PVectors at our current position
                // this makes our if statement a little cleaner
                Vector2 vc = p1[current];    // c for "current"
                Vector2 vn = p1[next];       // n for "next"

                // now we can use these two points (a line) to compare
                // to the other polygon's vertices using polyLine()
                bool collision = PolygonLine(p2, vc, vn);
                if (collision) return true;

                // optional: check if the 2nd polygon is INSIDE the first
                collision = PolygonPoint(p1, p2[0].X, p2[0].Y);
                if (collision) return true;
            }

            return false;
        }
        // TRIANGLE/POINT
        public static bool TrianglePoint(float x1, float y1, float x2, float y2, float x3, float y3, float px, float py)
        {

            // get the area of the triangle
            float areaOrig = Math.Abs((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1));

            // get the area of 3 triangles made between the point
            // and the corners of the triangle
            float area1 = Math.Abs((x1 - px) * (y2 - py) - (x2 - px) * (y1 - py));
            float area2 = Math.Abs((x2 - px) * (y3 - py) - (x3 - px) * (y2 - py));
            float area3 = Math.Abs((x3 - px) * (y1 - py) - (x1 - px) * (y3 - py));

            // if the sum of the three areas equals the original,
            // we're inside the triangle!
            if (area1 + area2 + area3 == areaOrig)
            {
                return true;
            }
            return false;
        }


        public static Vector2 LineLineIntersection(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
        {
            // Line AB represented as a1x + b1y = c1  
            float a1 = B.Y - A.Y;
            float b1 = A.X - B.X;
            float c1 = a1 * (A.X) + b1 * (A.Y);

            // Line CD represented as a2x + b2y = c2  
            float a2 = D.Y - C.Y;
            float b2 = C.X - D.X;
            float c2 = a2 * (C.X) + b2 * (C.X);

            float determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                // The lines are parallel. This is simplified  
                // by returning a pair of FLT_MAX  
                return new Vector2(float.MaxValue, float.MaxValue);
            }
            else
            {
                float x = (b2 * c1 - b1 * c2) / determinant;
                float y = (a1 * c2 - a2 * c1) / determinant;
                return new Vector2(x, y);
            }
        }
        public static Vector2 LineLineIntersection(Line line1, Line line2)
        {
            return LineLineIntersection(line1.A, line1.B, line2.A, line2.B);
        }
        public static Vector2 SegmentSegmentIntersection(Line lineA, Line lineB, out bool isContact)
        {
            float deltaACy = lineA.A.Y - lineB.A.Y;
            float deltaDCx = lineB.B.X - lineB.A.X;
            float deltaACx = lineA.A.X - lineB.A.X;
            float deltaDCy = lineB.B.Y - lineB.A.Y;
            float deltaBAx = lineA.B.X - lineA.A.X;
            float deltaBAy = lineA.B.Y - lineA.A.Y;

            float denominator = deltaBAx * deltaDCy - deltaBAy * deltaDCx;
            float numerator = deltaACy * deltaDCx - deltaACx * deltaDCy;

            if (denominator == 0)
            {
                if (numerator == 0)
                {
                    // collinear. Potentially infinite intersection points.
                    // Check and return one of them.
                    if (lineA.A.X >= lineB.A.X && lineA.A.X <= lineB.B.X)
                    {
                        isContact = true;
                        return lineA.A;
                    }
                    else if (lineB.A.X >= lineA.A.X && lineB.A.X <= lineA.B.X)
                    {
                        isContact = true;
                        return lineB.A;
                    }
                    else
                    {
                        isContact = false;
                        return Vector2.Zero;
                    }
                }
                else
                { // parallel
                    isContact = false;
                    return Vector2.Zero;
                }
            }

            double r = numerator / denominator;
            if (r < 0 || r > 1)
            {
                isContact = false;
                return Vector2.Zero;
            }

            double s = (deltaACy * deltaBAx - deltaACx * deltaBAy) / denominator;
            if (s < 0 || s > 1)
            {
                isContact = false;
                return Vector2.Zero;
            }

            isContact = true;
            return new Vector2((float)(lineA.A.X + r * deltaBAx), (float)(lineA.A.Y + r * deltaBAy));
        }

        // Find the point of intersection between
        // the lines p1 --> p2 and p3 --> p4.
        public static void FindIntersection(
            Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4,
            out bool lines_intersect, out bool segments_intersect,
            out Vector2 intersection,
            out Vector2 close_p1, out Vector2 close_p2)
        {
            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Vector2(float.NaN, float.NaN);
                close_p1 = new Vector2(float.NaN, float.NaN);
                close_p2 = new Vector2(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            float t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Vector2(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new Vector2(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new Vector2(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }

        public static void FindIntersection(
            Line line1, Line line2,
            out bool lines_intersect, out bool segments_intersect,
            out Vector2 intersection,
            out Vector2 close_p1, out Vector2 close_p2
            )
        {
            FindIntersection(
                line1.A, line1.B, line2.A, line2.B,
                out lines_intersect, out segments_intersect,
                out intersection,
                out close_p1, out close_p2
                );
        }
        public static Vector2 GetNormalPointLine(Vector2 A, Vector2 B, Vector2 C) // return Vec2 normal of C on line AB 
        {
            Vector2 AC, u, N;

            u.X = B.X - A.X;
            u.Y = B.Y - A.Y;

            AC.X = C.X - A.X;
            AC.Y = C.Y - A.Y;

            float parenthesis = u.X * AC.Y - u.Y * AC.X;  // calcul une fois pour les deux

            N.X = -u.Y * (parenthesis);
            N.Y = u.X * (parenthesis);

            // normalisons
            float norme = (float)Math.Sqrt(N.X * N.X + N.Y * N.Y);

            N.X /= norme;
            N.Y /= norme;

            return N;
        }
        public static Vector2 ProjectionPointLine(Vector2 A, Vector2 B, Vector2 C) // return Vec2 projection of C on line AB 
        {
            Vector2 u, AC;

            u.X = B.X - A.X;
            u.Y = B.Y - A.Y;

            AC.X = C.X - A.X;
            AC.Y = C.Y - A.Y;

            float ti = (u.X * AC.X + u.Y * AC.Y) / (u.X * u.X + u.Y * u.Y);

            Vector2 I;

            I.X = A.X + ti * u.X;
            I.Y = A.Y + ti * u.Y;

            return I;
        }
        public static bool PointInRect(Vector2 p, RectangleF r) // Test Point in Rect 
        {
            return (p.X > r.X && p.X < r.X + r.Width &&
                    p.Y > r.Y && p.Y < r.Y + r.Height);
        }
        public static bool PointInCircle(Vector2 p, CircleF c) // Test Point in Circle 
        {
            float d2 = (p.X - c.Center.X) * (p.X - c.Center.X) + (p.Y - c.Center.Y) * (p.Y - c.Center.Y);

            if (d2 > c.Radius * c.Radius)
                return false;
            else
                return true;
        }
        public static bool PointInCircle(Vector2 p, Vector2 center, float radius) // Test Point in Circle 
        {
            float d2 = (p.X - center.X) * (p.X - center.X) + (p.Y - center.Y) * (p.Y - center.Y);

            if (d2 > radius * radius)
                return false;
            else
                return true;
        }
        public static bool PointInEllipse(Vector2 test, Vector2 center, float rx, float ry)
        {
            float dx = test.X - center.X;
            float dy = test.Y - center.Y;

            return (dx * dx) / (rx * rx) + (dy * dy) / (ry * ry) <= 1;
        }
        public static bool PointInPolygonConvex(Vector2 P, Vector2[] pTab, int nbP, Vector2 offset = default)  // Test Point in Polygon Convexe 
        {
            for (int i = 0; i < nbP; ++i)
            {

                Vector2 A = pTab[i] + offset;
                Vector2 B;

                if (i == nbP - 1) // if last point join to first point
                    B = pTab[0] + offset;
                else            // else join to next point
                    B = pTab[i + 1] + offset;

                Vector2 D, T;

                D.X = B.X - A.X;
                D.Y = B.Y - A.Y;
                T.X = P.X - A.X;
                T.Y = P.Y - A.Y;

                float d = D.X * T.Y - D.Y * T.X;
                // if d < 0 P is at LEFT of vector AB
                // if d > 0 P is at RIGHT of vector AB
                // if d < 0 P is on vector AB
                if (d < 0)
                    return false;
            }
            return true;
        }
        public static int SegmentXSegment(Vector2 A, Vector2 B, Vector2 I, Vector2 P)
        {
            Vector2 D, E;

            D.X = B.X - A.X;
            D.Y = B.Y - A.Y;
            E.X = P.X - I.X;
            E.Y = P.Y - I.Y;

            double denom = D.X * E.Y - D.Y * E.X;

            if (denom == 0)
                return -1;   // ERROR , limit case

            float t = (float)(-(A.X * E.Y - I.X * E.Y - E.X * A.Y + E.X * I.Y) / denom);

            if (t < 0 || t >= 1)
                return 0;   // don't touch

            float u = (float)(-(-D.X * A.Y + D.X * I.Y + D.Y * A.X - D.Y * I.X) / denom);

            if (u < 0 || u >= 1)
                return 0;  // don't touch

            return 1;  // touch
        }
        public static bool LineSegment(Vector2 A, Vector2 B, Vector2 O, Vector2 P)
        {
            Vector2 AO, AP, AB;

            AB.X = B.X - A.X;
            AB.Y = B.Y - A.Y;
            AP.X = P.X - A.X;
            AP.Y = P.Y - A.Y;
            AO.X = O.X - A.X;
            AO.Y = O.Y - A.Y;

            if ((AB.X * AP.Y - AB.Y * AP.X) * (AB.X * AO.Y - AB.Y * AO.X) < 0)
                return true;
            else
                return false;
        }
        public static bool SegmentSegment(Vector2 A, Vector2 B, Vector2 O, Vector2 P)
        {
            if (!LineSegment(A, B, O, P))
                return false;
            if (!LineSegment(O, P, A, B))
                return false;

            return true;
        }
        public static bool SegmentSegmentEX(Vector2 A, Vector2 B, Vector2 O, Vector2 P)
        {
            if (LineSegment(A, B, O, P) == false)
                return false;  // inutile d'aller plus loin si le segment [OP] ne touche pas la droite (AB)

            Vector2 AB, OP;

            AB.X = B.X - A.X;
            AB.Y = B.Y - A.Y;
            OP.X = P.X - O.X;
            OP.Y = P.Y - O.Y;

            float k = -(A.X * OP.Y - O.X * OP.Y - OP.X * A.Y + OP.X * O.Y) / (AB.X * OP.Y - AB.Y * OP.X);

            if (k < 0 || k > 1)
                return false;
            else
                return true;
        }
        public static bool PointInPolygonConcave(Vector2 P, Vector2[] pTab, int nbP)
        {
            Vector2 I;

            Random random = new Random();

            I.X = 10000 + random.Next(0, 100);   // 10000 + un nombre aléatoire entre 0 et 99
            I.Y = 10000 + random.Next(0, 100);

            int nbintersections = 0;

            for (int i = 0; i < nbP; i++)
            {
                Vector2 A = pTab[i];
                Vector2 B;

                if (i == nbP - 1)  // si c'est le dernier point, on relie au premier
                    B = pTab[0];
                else           // sinon on relie au suivant.
                    B = pTab[i + 1];

                int iseg = SegmentXSegment(A, B, I, P);

                if (iseg == -1)
                    return PointInPolygonConcave(P, pTab, nbP);  // cas limite, on relance la fonction.

                nbintersections += iseg;
            }

            if (nbintersections % 2 == 1)  // nbintersections est-il impair ?
                return true;
            else
                return false;
        }
        public static bool LineCircle(Vector2 A, Vector2 B, CircleF C)
        {
            Vector2 u;
            u.X = B.X - A.X;
            u.Y = B.Y - A.Y;

            Vector2 AC;
            AC.X = C.Center.X - A.X;
            AC.Y = C.Center.Y - A.Y;

            float numerateur = u.X * AC.Y - u.Y * AC.X;   // norme du vecteur v

            if (numerateur < 0)
                numerateur = -numerateur;   // valeur absolue ; si c'est négatif, on prend l'opposé.

            float denominateur = (float)Math.Sqrt(u.X * u.X + u.Y * u.Y);  // norme de u

            float CI = numerateur / denominateur;

            if (CI < C.Radius)
                return true;
            else
                return false;
        }
        public static bool SegmentCircleF(Vector2 A, Vector2 B, CircleF C)
        {
            if (LineCircle(A, B, C) == false)
                return false;  // si on ne touche pas la droite, on ne touchera jamais le segment

            Vector2 AB, AC, BC;
            AB.X = B.X - A.X;
            AB.Y = B.Y - A.Y;
            AC.X = C.Center.X - A.X;
            AC.Y = C.Center.Y - A.Y;
            BC.X = C.Center.X - B.X;
            BC.Y = C.Center.Y - B.Y;

            float pscal1 = AB.X * AC.X + AB.Y * AC.Y;  // produit scalaire
            float pscal2 = (-AB.X) * BC.X + (-AB.Y) * BC.Y;  // produit scalaire

            if (pscal1 >= 0 && pscal2 >= 0)
                return true;   // I entre A et B, ok.

            // dernière possibilité, A ou B dans le cercle
            if (PointInCircle(A, C))
                return true;

            if (PointInCircle(B, C))
                return true;

            return false;
        }
        public static bool CircleCircle(CircleF c1, CircleF c2)
        {
            float d2 = (c1.Center.X - c2.Center.X) * (c1.Center.X - c2.Center.X) + (c1.Center.Y - c2.Center.Y) * (c1.Center.Y - c2.Center.Y);

            if (d2 > (c1.Radius + c2.Radius) * (c1.Radius + c2.Radius))
                return false;
            else
                return true;
        }
        public static bool RectRect(RectangleF r1, RectangleF r2)
        {
            if ((r2.X >= r1.X + r1.Width)    // trop à droite
                || (r2.X + r2.Width <= r1.X) // trop à gauche
                || (r2.Y >= r1.Y + r1.Height) // trop en bas
                || (r2.Y + r2.Height <= r1.Y))  // trop en haut
                return false;
            else
                return true;
        }

        // Make clip collide !
        //public static void MakeCollideNode(Node nodeA, Node nodeB)
        //{
        //    nodeA._isCollide = true;
        //    nodeB._isCollide = true;

        //    AddIndexCollideBy(nodeA, nodeB._index);
        //    AddIndexCollideBy(nodeB, nodeA._index);
        //}
        public static void MakeCollideZone(Collide.Zone zoneA, Collide.Zone zoneB)
        {
            if (zoneA._collideLevel == zoneB._collideLevel) // If same collideLevel then make them collide both !
            {
                zoneA._isCollide = true;
                zoneB._isCollide = true;

                zoneA._vecCollideBy.Add(zoneB);
                zoneB._vecCollideBy.Add(zoneA);

                return;
            }

            if (zoneA._collideLevel > zoneB._collideLevel) // If different collideLevel then make only lowest collideLevel as collide !
            {
                zoneB._isCollide = true;
                zoneB._vecCollideBy.Add(zoneA);
            }
            else
            {
                zoneA._isCollide = true;
                zoneA._vecCollideBy.Add(zoneB);
            }

        }
        public static void AddIndexCollideBy(Node node, int id) // Avoid collision duplication
        {
            node._vecCollideBy.Add(id);
        }

        //----------------------------------------------------------------------------------------------------------
        // Begin On Collide By
        //----------------------------------------------------------------------------------------------------------

        // With Only One Node
        //public static int OnCollideByName(Node node, string name)
        //{
        //    if (node._vecCollideBy.Count > 0)
        //        for (int i = 0; i< node._vecCollideBy.Count; i++)
        //        {
        //            if (null != node._parent.Index(i))
        //                if (node._parent.Index(i)._name == name)
        //                {
        //                    node._idCollideName = i;
        //                    return i;
        //                }
        //        }

        //    node._idCollideName = -1;
        //    return -1;
        //}
        //public static int OnCollideByIndex(Node node, int id)
        //{
        //    if (node._vecCollideBy.Count > 0)
        //        for (int i = 0; i < node._vecCollideBy.Count; i++)
        //        {
        //            if (null != node._parent.Index(i))
        //                if (node._parent.Index(i)._index == id)
        //                {
        //                    node._idCollideIndex = i;
        //                    return i;
        //                }
        //        }

        //    node._idCollideIndex = -1;
        //    return -1;
        //}
        //public static int OnCollideByType(Node node, int type)
        //{
        //    if (node._vecCollideBy.Count > 0)
        //        for (int i = 0; i < node._vecCollideBy.Count; i++)
        //        {
        //            if (null != node._parent.Childs().At(i))
        //                if (node._parent.Childs().At(i)._type == type)
        //                {
        //                    node._idCollideIndex = i;
        //                    return i;
        //                }
        //        }

        //    node._idCollideIndex = -1;
        //    return -1;
        //}

        // With Only One Node Zone 
        public static Collide.Zone? OnCollideZoneByNodeName(Collide.Zone zone, string name, int indexZone)
        {
            if (zone._vecCollideBy.Count > 0)
                for (int i = 0; i < zone._vecCollideBy.Count; i++)
                {
                    Collide.Zone it = zone._vecCollideBy.ElementAt(i);
                    if (null != it)
                        if (it._node._name == name)
                        {
                            if (it._index == indexZone)
                            {
                                zone._zoneCollideBy = it;
                                return it;
                            }
                        }
                }

            //zone->_zoneCollideBy = nullptr;
            return null;
        }
        public static Collide.Zone? OnCollideZoneByNodeIndex(Collide.Zone zone, int id, int indexZone)
        {
            if (zone._vecCollideBy.Count > 0)
                for (int i = 0; i < zone._vecCollideBy.Count; i++)
                {
                    Collide.Zone it = zone._vecCollideBy.ElementAt(i);
                    if (null != it)
                        if (it._node._index == id)
                        {
                            if (it._index == indexZone)
                            {
                                zone._zoneCollideBy = it;
                                return it;
                            }
                        }
                }

            //zone->_zoneCollideBy = nullptr;
            return null;
        }
        public static Collide.Zone? OnCollideZoneByNodeType(Collide.Zone zone, int type, int indexZone)
        {
            if (zone._vecCollideBy.Count > 0)
                for (int i = 0; i < zone._vecCollideBy.Count; i++)
                {
                    Collide.Zone it = zone._vecCollideBy.ElementAt(i);
                    if (null != it)
                        if (it._node._type == type)
                        {
                            if (it._index == indexZone)
                            {
                                zone._zoneCollideBy = it;
                                return it;
                            }
                        }
                }

            //zone->_zoneCollideBy = nullptr;
            return null;
        }


        // With list Node Zone // Get First Zone
        public static Collide.Zone? OnCollideZoneByNodeType(Collide.Zone zone, int[] types, int indexZone)
        {
            if (zone._vecCollideBy.Count > 0)
                for (int i = 0; i < zone._vecCollideBy.Count; i++)
                {
                    Collide.Zone it = zone._vecCollideBy.ElementAt(i);
                    if (null != it)
                        for (int t = 0; t < types.Length; t++)
                            if (it._node._type == types[t])
                            {
                                if (it._index == indexZone)
                                {
                                    zone._zoneCollideBy = it;
                                    return it;
                                }
                            }
                }

            return null;
        }
        public static Collide.Zone? OnCollideZoneByNodeType(Collide.Zone zone, int[] types, int[] indexZones)
        {
            // Error return null if list types Length is different than indexZone
            if (types.Length != indexZones.Length) return null;

            if (zone._vecCollideBy.Count > 0)
                for (int i = 0; i < zone._vecCollideBy.Count; i++)
                {
                    Collide.Zone it = zone._vecCollideBy.ElementAt(i);
                    if (null != it)
                        for (int t = 0; t < types.Length; t++)
                            if (it._node._type == types[t])
                            {
                                if (it._index == indexZones[t])
                                {
                                    zone._zoneCollideBy = it;
                                    return it;
                                }
                            }
                }

            return null;
        }

        // Get List of Collide Zone 
        public static List<Collide.Zone> ListCollideZoneByNodeType(Collide.Zone zone, int type, int indexZone)
        {
            List<Collide.Zone> collideZones = new List<Collide.Zone>();

            if (zone._vecCollideBy.Count > 0)
            {
                for (int i = 0; i < zone._vecCollideBy.Count; i++)
                {
                    Collide.Zone it = zone._vecCollideBy.ElementAt(i);
                    if (null != it)
                        if (it._node._type == type)
                        {
                            if (it._index == indexZone)
                            {
                                zone._zoneCollideBy = it;

                                collideZones.Add(it);
                                //return it;
                            }
                        }
                }

            }

            return collideZones;
        }
        public static List<Collide.Zone> ListCollideZoneByNodeType(Collide.Zone zone, int[] types, int indexZone)
        {
            List<Collide.Zone> collideZones = new List<Collide.Zone>();

            if (zone._vecCollideBy.Count > 0)
            {
                for (int i = 0; i < zone._vecCollideBy.Count; i++)
                {
                    Collide.Zone it = zone._vecCollideBy.ElementAt(i);
                    if (null != it)
                        for (int t = 0; t < types.Length; t++)
                            if (it._node._type == types[t])
                            {
                                if (it._index == indexZone)
                                {
                                    zone._zoneCollideBy = it;

                                    collideZones.Add(it);
                                    //return it;
                                }
                            }
                }

            }

            return collideZones;
        }
        public static List<Collide.Zone>? ListCollideZoneByNodeType(Collide.Zone zone, int[] types, int[] indexZones)
        {
            // Error return null if list types Length is different than indexZone
            if (types.Length != indexZones.Length) return null;

            List<Collide.Zone> collideZones = new List<Collide.Zone>();

            if (zone._vecCollideBy.Count > 0)
            {
                for (int i = 0; i < zone._vecCollideBy.Count; i++)
                {
                    Collide.Zone it = zone._vecCollideBy.ElementAt(i);
                    if (null != it)
                        for (int t = 0; t < types.Length; t++)
                            if (it._node._type == types[t])
                            {
                                if (it._index == indexZones[t])
                                {
                                    zone._zoneCollideBy = it;

                                    collideZones.Add(it);
                                    //return it;
                                }
                            }
                }

            }

            return collideZones;
        }

        //----------------------------------------------------------------------------------------------------------
        // End On Collide By
        //----------------------------------------------------------------------------------------------------------

        // Collisions check !
        //public static void ResetAllNode(Node node)
        //{
        //    for (int i = 0; i < node.NbNode(); ++i)
        //    {
        //        if (null != node.Index(i))
        //        {
        //            node.Index(i)._isCollide = false;
        //            node.Index(i)._idCollideName = -1;
        //            node.Index(i)._idCollideIndex = -1;
        //            node.Index(i)._vecCollideBy.Clear();
        //        }
        //    }
        //}
        public static void ResetAllZone(Node node)
        {
            // Iterate all Child Clip
            for (int i = 0; i < node.NbNode(); ++i)
            {
                // If Clip is Valid
                if (null != node.Index(i))
                    // If Clip is Collidable
                    //if (node.Index(i)._isCollidable &&
                    if (node.Index(i)!._collideZones.Count > 0)
                    // Iterate All CollideZone of this Clip
                    {
                        foreach (KeyValuePair<int, Collide.Zone> zone in node.Index(i)!._collideZones)
                        {
                            if (null != zone.Value)
                            {
                                zone.Value._isCollide = false;
                                //zone.Value._isCollidable = false;
                                zone.Value._vecCollideBy.Clear();
                            }
                        }
                    }
            }
        }

        // Collision System !
        //public static void BruteSystemNode(Node node)
        //{

        //    for (int i = 0; i < node.NbNode(); ++i)
        //    {
        //        for (int j = i + 1; j < node.NbNode(); ++j)
        //        {

        //            if (null != node.Index(i) &&
        //                null != node.Index(j))
        //            {
        //                if (node.Index(i)._isCollidable &&
        //                    node.Index(j)._isCollidable)
        //                {
        //                    if (RectRect(node.Index(i)._rect,
        //                                 node.Index(j)._rect))
        //                    {
        //                        MakeCollideNode(node.Index(i), node.Index(j));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //public static void GridSystemNode(Node node, Collision2DGrid grid)
        //{
        //    grid.ClearAll();

        //    for (int i = 0; i < node.NbNode(); ++i)
        //    {
        //        if (null != node.Index(i))
        //            grid.InsertClipInCell(i, node.Index(i)._rect, node.Index(i));
        //    }

        //    //for (unsigned i = 0; i < grid->_vecCollideZone.size(); ++i)
        //    for (int i = 0; i < node.NbNode(); ++i)
        //    {
        //        if (null != node.Index(i))
        //        {
        //            grid._vecCollideZone.Clear();
        //            grid.FindNear(grid._vecCollideZone, node.Index(i)._rect);

        //            if (grid._vecCollideZone.Count>0)
        //            {
        //                for (int x = 0; x < grid._vecCollideZone.Count; ++x)
        //                {
        //                    int j = grid._vecCollideZone[x]._index;
        //                    if (i != j)
        //                    {
        //                        if (j >= 0 && j < node.NbNode())
        //                        {
        //                            if (node.Index(i)._isCollidable &&
        //                                node.Index(j)._isCollidable)
        //                            {
        //                                if (RectRect(node.Index(i)._rect,
        //                                             node.Index(j)._rect))
        //                                {
        //                                    MakeCollideNode(node.Index(i), node.Index(j));
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //    }
        //}
        public static void GridSystemZone(Node node, Collision2DGrid grid)
        {
            grid.ClearAll();

            // Iterate all Child Clip
            for (int i = 0; i < node.NbNode(); ++i)
            {
                // If Clip is Valid
                if (null != node.Index(i))
                    // If Clip is Collidable
                    //if (node.Index(i)._collideZones.Count>0 && node.Index(i)._isCollidable)
                    if (node.Index(i)!._collideZones.Count > 0 && node.Index(i)!._isActive)
                    {
                        // Iterate All CollideZone of this Clip
                        //auto it = clip->index(i)->_mapCollideZone.begin();
                        //while (it != clip->index(i)->_mapCollideZone.end())
                        foreach (KeyValuePair<int, Collide.Zone> it in node.Index(i)!._collideZones)
                        {
                            if (null != it.Value)
                            {
                                if (it.Value._isCollidable) // ----- Optimization  Insert only if is true !
                                    grid.InsertZoneInCell(it.Value);
                            }
                        }
                    }
            }

            // Iterate all Child Clip
            for (int i = 0; i < node.NbNode(); ++i)
            {
                // If Clip is Valid
                if (null != node.Index(i) && node.Index(i)!._isActive)
                {
                    //if (node.Index(i)._collideZones.Count>0 && node.Index(i)._isCollidable)
                    if (node.Index(i)!._collideZones.Count > 0)
                    {
                        grid._collideZones.Clear();

                        // Iterate All CollideZone of this Clip
                        //auto iti = clip->index(i)->_mapCollideZone.begin();
                        //while (iti != clip->index(i)->_mapCollideZone.end())
                        foreach (KeyValuePair<int, Collide.Zone> iti in node.Index(i)!._collideZones)
                        {

                            if (iti.Value._isCollidable) // ----- Optimization  Test collision only if is true !
                            {
                                // Search Zone near other Zone ! in the same Case
                                grid.FindNear(grid._collideZones, iti.Value);

                                // If find near Zone !
                                if (grid._collideZones.Count > 0)
                                {
                                    // Iterate all found near Zone
                                    //for (auto & itj: grid->_vecCollideZone)
                                    foreach (Collide.Zone itj in grid._collideZones)
                                    {
                                        // Test iti != itj // avoid comparaison with the same Zone
                                        if (iti.Value._node._index != itj._node._index)
                                        {
                                            if (itj._isCollidable)
                                                // Test Collision
                                                if (RectRect(iti.Value._rect, itj._rect))
                                                {
                                                    MakeCollideZone(iti.Value, itj);
                                                    //                                        std::cout << "-Hit- \n";
                                                    //                                        printf("Zone1 : %s  id = %i \n",
                                                    //                                               iti->second->_clip->_name.c_str(),
                                                    //                                               iti->second->_clip->id());
                                                    //
                                                    //                                        printf("Zone2 : %s  id = %i \n",
                                                    //                                               itj->_clip->_name.c_str(),
                                                    //                                               itj->_clip->id());

                                                }
                                        }
                                    }
                                }

                            }


                        }
                    }
                }
            }
        }
    }
}
