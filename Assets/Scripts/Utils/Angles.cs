namespace Utils {
    public class Angles {
        public static float Mod(float x, int m) {
            float r = x%m;
            return r<0 ? r+m : r;
        }
    }
}