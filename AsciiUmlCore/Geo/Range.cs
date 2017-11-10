namespace AsciiUml.Geo {
	public class Range<T> {
		public T Min, Max;

		public Range(T min, T max) {
			Min = min;
			Max = max;
		}

		public override string ToString() {
			return $"{Min} - {Max}";
		}
	}
}