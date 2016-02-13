public class SEFix {
    public static T[] arr<T>(params T[] arg) {
        return arg; //becuse SE is stupid
    }
}
public class ColorUtils {
    private static double oo64 = 1.0/64.0;
    private static double[][] map = new double[][] {
        new double[] { 0*oo64, 48*oo64, 12*oo64, 60*oo64,  3*oo64, 51*oo64, 15*oo64, 63*oo64},
        new double[] {32*oo64, 16*oo64, 44*oo64, 28*oo64, 35*oo64, 19*oo64, 47*oo64, 31*oo64},
        new double[] { 8*oo64, 56*oo64,  4*oo64, 52*oo64, 11*oo64, 59*oo64,  7*oo64, 55*oo64},
        new double[] {40*oo64, 24*oo64, 36*oo64, 20*oo64, 43*oo64, 27*oo64, 39*oo64, 23*oo64},
        new double[] { 2*oo64, 50*oo64, 14*oo64, 62*oo64,  1*oo64, 49*oo64, 13*oo64, 61*oo64},
        new double[] {34*oo64, 18*oo64, 46*oo64, 30*oo64, 33*oo64, 17*oo64, 45*oo64, 29*oo64},
        new double[] {10*oo64, 58*oo64,  6*oo64, 54*oo64,  9*oo64, 57*oo64,  5*oo64, 53*oo64},
        new double[] {42*oo64, 26*oo64, 38*oo64, 22*oo64, 41*oo64, 25*oo64, 37*oo64, 21*oo64}
    };
    
    private static int[][] palette = new int[][] {
        SEFix.arr( 255, 255, 0),
        SEFix.arr( 255, 0, 0),
        SEFix.arr( 0, 0, 255),
        SEFix.arr( 0, 255, 0),
        SEFix.arr( 255, 255, 255),
        SEFix.arr( 97, 97, 97),
        SEFix.arr( 0, 0, 0)
    };
    private static string[] colorStrings = new string[] {
        "\uE004", //oh but it works fine with *strings* -_-
        "\uE003",
        "\uE002",
        "\uE001",
        "\uE007\u0458",
        "\uE00D",
        "\u2014\u0060"
    };
    
    private static int redC = 300;
    private static int greenC = 540;
    private static int blueC = 150;
    
    private static double compareColors(int r1, int g1, int b1, int r2, int g2, int b2) {
        double dl = ((r1*redC + g1*greenC + b1*blueC)-(r2*redC + g2*greenC + b2*blueC))/255000.0;
        double dr = (r1-r2)/255.0, dg = (g1-g2)/255.0, db = (b1-b2)/255.0;
        return ((dr*dr*redC + dg*dg*greenC + db*db*blueC)*0.0075 + dl*dl);
    }
    private static double calcError(int[] color, int r0, int g0, int b0, int[] color1, int[] color2, double ratio) {
        return compareColors(color[0], color[1], color[2], r0,g0,b0) +
            compareColors(color1[0], color1[1], color1[2], color2[0], color2[1], color2[2]) *0.03*(Math.Abs(ratio-0.5)+0.5)* 
            (1+ (color1[0]==color1[1] && color1[0]==color1[2] && color1[0]==color2[0] && 
             color1[0] == color2[1] && color1[0]==color2[2] ? 0.03 : 0));
    }
    public static void makeRatio(int[] c, int[] c1, int[] c2, out int ratio) {
        ratio = 32;
        if (c1[0] != c2[0] || c1[1] != c2[1] || c1[2] != c2[2])
            ratio =
                ((c2[0] != c1[0] ? redC  *64 * (c[0] - c1[0]) / (c2[0]-c1[0]) : 0) +
                 (c2[1] != c1[1] ? greenC*64 * (c[1] - c1[1]) / (c2[1]-c1[1]) : 0) +
                 (c1[2] != c2[2] ? blueC *64 * (c[2] - c1[2]) / (c2[2]-c1[2]) : 0))/
                ((c2[0] != c1[0] ? redC   : 0)+
                 (c2[1] != c1[1] ? greenC : 0)+
                 (c2[2] != c1[2] ? blueC  : 0));
        if (ratio < 0)
            ratio = 0;
        else if (ratio > 63)
            ratio = 63;
    }
    public static void createMix(int[] result, int[] rgb) {
        result[0] = 0; result[1] = 0; result[2] = 32;
        double minPenalty = Single.MaxValue;
        int ratio;
        for (int i = 0; i < palette.Length; i++) {
            for (int j = i; j < palette.Length; j++) {
                makeRatio(rgb, palette[i], palette[j], out ratio);
                double penalty = calcError(
                    rgb,
                    palette[i][0] + ratio * (palette[j][0]-palette[i][0]) / 64,
                    palette[i][1] + ratio * (palette[j][1]-palette[i][1]) / 64,
                    palette[i][2] + ratio * (palette[j][2]-palette[i][2]) / 64,
                    palette[i], palette[j],
                    (double)ratio/64.0);
                if (penalty < minPenalty) {
                    minPenalty = penalty;
                    result[0] = i;
                    result[1] = j;
                    result[2] = ratio;
                }
            }
        }
    }
    public static void genDitherPattern(string[][] dithered, int[] mix) {
        for (int x = 0; x < 8; x++) {
            dithered[x] = new string[8];
            for (int y = 0; y < 8; y++) {
                double mapValue = map[y&7][x&7];
                double ratio = mix[2]/64.0;
                dithered[x][y] = colorStrings[mix[ mapValue < ratio ? 1 : 0 ]];
            }
        }
    }
}
public class Ascii {
    private static int offset = 0x21;
    // binary numbers represent bitmap glyphs, three bits to a line
    // eg 9346 == 010 010 010 000 010 == !
    //    5265 == 001 010 010 010 001 == (
    private static short[] glyphs = SEFix.arr<short>(
        9346, 23040, 24445, 15602,
        17057, 10923, 9216, 5265,
        17556, 21824, 1488, 20,
        448, 2, 672, 31599,
        11415, 25255, 29326, 23497,
        31118, 10666, 29370, 10922,
        10954, 1040, 1044, 5393,
        3640, 17492, 25218, 15203,
        11245, 27566, 14627, 27502,
        31143, 31140, 14827, 23533,
        29847, 12906, 23469, 18727,
        24557, 27501, 11114, 27556,
        11131, 27565, 14478, 29842,
        23403, 23378, 23549, 23213,
        23186, 29351, 13459, 2184,
        25750, 10752, 7, 17408,
        239, 18862, 227, 4843,
        1395, 14756, 1886, 18861,
        8595, 4302, 18805, 25745,
        509, 429, 170, 1396,
        1369, 228, 1934, 18851,
        363, 362, 383, 341,
        2766, 3671, 5521, 9234,
        17620, 1920
    );
    public static short getGlyph(char code) {
        return glyphs[code-offset];
    }
}
public class Graphics {
    public readonly int width;
    public readonly int height;
    private IMyTextPanel console;
    private string[] screen;
    private string[] screenLines;
    public int[] bgc;
    public int[] fgc;
    private int[] fgMix;
    private int[] bgMix;
    private string[][] fgDither;
    private string[][] bgDither;
    private int[] clip;
    public Graphics(int w, int h, IMyTextPanel c) {
        width = w;
        height = h;
        console = c;
        
        screen = new string[height*width];
        screenLines = new string[width*height+height-1];
        
        clip = new int[4];
        mask();
        fgc = new int[3];
        bgc = new int[3];
        fgMix = new int[3];
        bgMix = new int[3];
        fgDither = new string[8][];
        bgDither = new string[8][];
        setFG(255,255,255);
        setBG(0,0,0);
    }
    public void setFG(int r, int g, int b) {
        fgc[0] = r;
        fgc[1] = g;
        fgc[2] = b;
        ColorUtils.createMix(fgMix, fgc);
        ColorUtils.genDitherPattern(fgDither, fgMix);
    }
    public void setBG(int r, int g, int b) {
        bgc[0] = r;
        bgc[1] = g;
        bgc[2] = b;
        ColorUtils.createMix(bgMix, bgc);
        ColorUtils.genDitherPattern(bgDither, bgMix);
    }
    public void mask(int x1, int y1, int x2, int y2) {  
        clip[0] = x1; 
        clip[1] = y1; 
        clip[2] = x2; 
        clip[3] = y2; 
    }
    public void mask() {
        clip[0] = 0;
        clip[1] = 0;
        clip[2] = width-1;
        clip[3] = height-1;
    }
    public void paint() {
        for (int i=0; i < height; i++) {
            screenLines[i] = string.Join(null, screen, i*width, width)+"\n";
        }
        console.WritePublicText(string.Concat( screenLines));
        console.ShowTextureOnScreen();
        console.ShowPublicTextOnScreen();
    }
    public void clear() {
        for (int i=0; i < 8; i++) {
            for (int j=0; j < width; j+=8) {
                Array.Copy(bgDither[i], 0, screen, i*width+j, 8);
            }
        }
        int size = width*height;
        int half = width*height >> 1;
        for (int i = width*8; i < size; i *= 2) {
            int copyLength = i;
            if (i > half) {
                copyLength = size - i;
            }
            Array.Copy(screen, 0, screen, i, copyLength);
        }
    }
    public void pixel(int x, int y) {
        if (x >= clip[0] && x <= clip[2] && y >= clip[1] && y <= clip[3]) {
            screen[width*y + x] = fgDither[x&7][y&7];
        }
    }
    public void line(int x0, int y0, int x1, int y1) {
        if (x0 == x1) {
            int high = Math.Max(y1,y0);
            for (int y = Math.Min(y1,y0); y <= high; y++) {
                pixel(x0,y);
            }
        } else if (y0 == y1) {
            int high = Math.Max(x1,x0);
            for (int x = Math.Min(x1,x0); x <= high; x++) {
                pixel(x,y0);
            }
        } else {
            bool yLonger=false;
            int incrementVal, endVal;
            int shortLen=y1-y0;
            int longLen=x1-x0;
            if (Math.Abs(shortLen)>Math.Abs(longLen)) {
                int swap=shortLen;
                shortLen=longLen;
                longLen=swap;
                yLonger=true;
            }
            endVal=longLen;
            if (longLen<0) {
                incrementVal=-1;
                longLen=-longLen;
            } else incrementVal=1;
            int decInc;
            if (longLen==0) decInc=0;
            else decInc = (shortLen << 16) / longLen;
            int j=0;
            if (yLonger) {
                for (int i=0;i-incrementVal!=endVal;i+=incrementVal) {
                    pixel(x0+(j >> 16),y0+i);     
                    j+=decInc;
                }
            } else {
                for (int i=0;i-incrementVal!=endVal;i+=incrementVal) {
                    pixel(x0+i,y0+(j >> 16));
                    j+=decInc;
                }
            }
        }
    }
    public void rect(string m, int xb, int yb, int w, int h) {
        if (m == "line") {
            line(xb, yb, xb, yb+h-1);
            line(xb, yb, xb+w-1, yb);
            line(xb+w-1, yb, xb+w-1, yb+h-1);
            line(xb, yb+h-1, xb+w-1, yb+h-1);
        } else if (m == "fill") {
            for (int x = xb; x < xb+w; x++) {
                for (int y = yb; y < yb+h; y++) {
                    pixel(x,y);
                }
            }
        }
    }
    public void ellipse(string m, int cx, int cy, int rx, int ry) {
        int rx2 = rx*rx;
        int ry2 = ry*ry;
        if (m == "fill") {
            int rxsys = rx2*ry2;
            pixel(cx, cy);
            for (int i=1; i < rx*ry; i++) {
                int x = i % rx;
                int y = i / rx;
                if (ry2*x*x+rx2*y*y <= rxsys) {
                    pixel(cx+x, cy+y);
                    pixel(cx-x, cy-y);
                    //if (x && y) { //unnecessary (prevents overdrawing pixels)
                    pixel(cx+x, cy-y);
                    pixel(cx-x, cy+y);
                    //}
                }
            }
        } else if (m == "line") {
            int frx2 = 4 * rx2;
            int fry2 = 4 * ry2;
            int s = 2*ry2+rx2*(1-2*ry);
            int y = ry;
            for (int x = 0; ry2*x <= rx2*y; x++) {
                pixel(cx + x, cy + y);
                pixel(cx - x, cy + y);
                pixel(cx + x, cy - y);
                pixel(cx - x, cy - y);
                if (s >= 0) {
                    s += frx2 * (1 - y);
                    y--;
                }
                s += ry2 * ((4 * x) + 6);
            }
            y = 0;
            s = 2*rx2+ry2*(1-2*rx);
            for (int x = rx; rx2*y <= ry2*x; y++) {
                pixel(cx + x, cy + y);
                pixel(cx - x, cy + y);
                pixel(cx + x, cy - y);
                pixel(cx - x, cy - y);
                if (s >= 0) {
                    s += fry2 * (1 - x);
                    x--;
                }
                s += rx2 * ((4 * y) + 6);
            }
        }
    }
    public void circle(string m, int cx, int cy, int r) {
        if (m == "fill") {
            int rr = r*r;
            pixel(cx, cy);
            for (int i=1; i < r*r; i++) {
                int x = i % r;
                int y = i / r;
                if (x*x+y*y < rr) {
                    pixel(cx+x, cy+y);
                    pixel(cx-x, cy-y);
                    if (x>0 && y>0) {
                        pixel(cx+x, cy-y);
                        pixel(cx-x, cy+y);
                    }
                }
            }
        } else if (m == "line") {
            int x = r;
            int y = 0;
            int do2 = 1 - x;
            while (y <= x) {
                pixel(cx+x, cy+y);
                pixel(cx+y, cy+x);
                pixel(cx-x, cy+y);
                pixel(cx-y, cy+x);
                pixel(cx-x, cy-y);
                pixel(cx-y, cy-x);
                pixel(cx+x, cy-y);
                pixel(cx+y, cy-x);
                y++;
                if (do2 <= 0) {
                    do2 += 2 * y + 1;
                } else {
                    do2 += 2 * (y - --x) + 1;
                }
            }
        }
    }
    public void print(int x, int y, string text) {
        int x1 = x;
        int y1 = y;
        for (int i = 0; i < text.Length; i++) {
            switch(text[i]) {
                case '\n':
                    y1 += 6;
                    x1 = x;
                    break;
                case ' ':
                    x1 += 4;
                    break;
                default:
                    short glyph = Ascii.getGlyph(text[i]);
                    int j = 14;
                    do {
                        if ((glyph & 1) != 0) {
                            pixel(x1+j%3, y1-4+j/3);
                        }
                        glyph >>= 1;
                        j--;
                    } while (glyph > 0);
                    x1 += 4;
                    break;
            }
        }
    }
}

/*----------------------------------------------------------------------------------------------------------*/
//END GRAPHICS LIBRARY IMPLEMENTATION//
/*----------------------------------------------------------------------------------------------------------*/

const int SCREENWIDTH = 300;
const int SCREENHEIGHT = 150;
const int FRAMES_PER_SECOND = 2; //20 fps attempts to run at 1 frame per tick and is severely laggy when running graphics scripts
const int PADDLEW = 10;
const int PADDLEH = 50;

public class Paddle{
    public int x;
    public int y;
    private int speed;
    private Graphics graphics;

    public Paddle(int xpos, int ypos, int pSpeed, Graphics g){
        x = xpos;
        y = ypos;
        speed = pSpeed;
        graphics = g;
    }

    public void movePaddle(bool upPressed, bool downPressed){
        if(upPressed && y > 0){
            y += -1*speed;
        }
        if(downPressed && y < SCREENHEIGHT){
            y += speed;
        }
    }

    public void movePaddleAI(Ball ball){
        int ballY = ball.y;

        if(y > ballY + 2){
            movePaddle(true, false);
        }
        else if(y < ballY - 2){
            movePaddle(false, true);
        }
    }

    public void drawPaddle(){
        graphics.rect("fill", x, y, PADDLEW, PADDLEH);
    }
}

public class Ball{
    public int x;
    public int y;
    public int xVel = -1;
    public int yVel = 2;
    private Graphics graphics;

    public Ball(int xpos, int ypos, Graphics g){
        x = xpos;
        y = ypos;
        graphics = g;
    }

    public int moveBall(Paddle p1, Paddle p2){
        x += xVel;
        y += yVel;

        return checkCollisions(p1, p2);
    }

    public int checkCollisions(Paddle p1, Paddle p2){
        int p1x = p1.x;
        int p1y = p1.y;
        int p2x = p2.x;
        int p2y = p2.y;

        if(x < SCREENWIDTH/4){
            if( x >= p1x && x <= p1x + PADDLEW && y >= p1y && y <= p1y + PADDLEH){
                //colliding with p1
                xVel = xVel * -1;
            }
            else if(x <= 0){
                resetPos();
                return 2;
            }
        }
        else if(x > 3*(SCREENWIDTH/4)){
            if( x >= p2x && x <= p2x + PADDLEW && y >= p2y && y <= p2y + PADDLEH){
                //colliding with p2
                xVel = xVel * -1;
            }
            else if(x >= SCREENWIDTH){
                resetPos();
                return 1;
            }
        }

        if(y >= SCREENHEIGHT || y <= 0){
            yVel = yVel * -1;
        }

        return 0;

    }

    public void resetPos(){
        x = SCREENWIDTH/2;
        y = SCREENHEIGHT/2;
    }

    public void drawBall(){
        graphics.circle("fill", x, y, 4);
    }
}

public class Pong{
    public Paddle paddle1;
    public Paddle paddle2;
    public Ball ball;
    public int p1Score = 0;
    public int p2Score = 0;
    private Graphics graphics;

    public Pong(Graphics g){
        graphics = g;
        paddle1 = new Paddle((SCREENWIDTH/8)-5, (SCREENHEIGHT/2)-25, 1, graphics);
        paddle2 = new Paddle((7*(SCREENWIDTH/8))-5, (SCREENHEIGHT/2)-25, 1, graphics);
        ball = new Ball(SCREENWIDTH/2, SCREENHEIGHT/2, graphics);
    }

    public void drawGame(){
        graphics.clear();
        graphics.setFG(255,255,255);
        graphics.print(SCREENWIDTH/4, SCREENHEIGHT/8, p1Score.ToString());
        graphics.print(3*(SCREENWIDTH/4), SCREENHEIGHT/8, p2Score.ToString());
        paddle1.drawPaddle();
        paddle2.drawPaddle();
        ball.drawBall();
        graphics.paint();
    }

    public void moveEntities(bool upPressed, bool downPressed){
        paddle1.movePaddle(upPressed, downPressed);
        paddle2.movePaddleAI(ball);
        int scoreUpdate = ball.moveBall(paddle1, paddle2);
        updateScore(scoreUpdate);

    }

    public void updateScore(int scoreUpdate){
        switch(scoreUpdate){
            case 1:
                p1Score += 1;
                break;
            case 2:
                p2Score += 1;
                break;
            default:
                break;
        }
    }

    public void tick(){
        drawGame();
        moveEntities(false, false);
    }
    
    public void setGraphics(Graphics g){
        graphics = g;
    }
}



Graphics graphics;
int time = 0;
int frameModulo = 20/FRAMES_PER_SECOND;
Pong game;

void Main(string argument)
{

    if(time++ % frameModulo != 0){
        return;
    }


    if(graphics == null){
        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(blocks, null);
        graphics = new Graphics(SCREENWIDTH, SCREENHEIGHT, (IMyTextPanel)blocks[0]);
        game = new Pong(graphics);
        //A widescreen lcd with font size 0.1 takes up approximately 321x178 "pixels" with the drawing library
        //so I rounded down to 320 and 175, there's a bit of blank space on the far right and bottom edges that
        //aren't covered by the graphics canvas
    }

    game.tick();

    /*
    graphics.circle("line", 60,60,60);
    for (int min=0; min < 720; min += 15) {
        double angle = min * Math.PI/360;
        int imp = min%60==0 ? 4 : 2;
        double cosA = Math.Cos(angle);
        double sinA = Math.Sin(angle);
        graphics.line(
            60+(int)((60-imp)*cosA),
            60+(int)((60-imp)*sinA),
            60+(int)(60*cosA),
            60+(int)(60*sinA)
        );
        if (imp==4) {
            int x3 = 60+(int)(50*cosA);
            int y3 = 60+(int)(50*sinA);
            graphics.print(x3,y3,""+((min/60+2)%12+1));
        }
    }
    DateTime t = DateTime.Now;
    double S = (t.Second-15)%60;
    double M = (t.Minute-15)%60;
    double H = (t.Hour-3)%12;
    double SA =  S * Math.PI/30;
    double MA =  M * Math.PI/30;
    double HA = Math.PI/360 * (60*H+M);

    graphics.line(60,60, 60+(int)(Math.Cos(HA)*30),60+(int)(Math.Sin(HA)*30));
    graphics.line(60,60, 60+(int)(Math.Cos(MA)*45),60+(int)(Math.Sin(MA)*45));
    graphics.setFG(255,0,0);
    graphics.line(60,60, 60+(int)(Math.Cos(SA)*45),60+(int)(Math.Sin(SA)*45));
    */

}