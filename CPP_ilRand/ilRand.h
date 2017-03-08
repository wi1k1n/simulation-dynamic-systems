#pragma once

#define ulonglong unsigned long long


namespace Diploma2 {

	class ilRand
	{
	public:
		int seed;
		ulonglong x = 0;

		ilRand();
		ilRand(int seed);
		ilRand(int seed, int state);
		~ilRand();

		int Next();
		int Next(int max);
		int Next(int min, int max);

		double NextDouble();
		double NextDouble(double max);
		double NextDouble(double min, double max);

		void Initialize();
		void Initialize(int seed);
		void Initialize(int seed, int state);
	private:
		const ulonglong m = 281474976710655;
		const ulonglong a = 25214903917;
		const ulonglong c = 11;
		ulonglong next();
	};


}