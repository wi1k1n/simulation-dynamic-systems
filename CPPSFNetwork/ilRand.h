#pragma once

#define ulonglong unsigned long long

class ilRand
{
public:
	ilRand();
	ilRand(int seed);
	~ilRand();

	int Next();
	int Next(int max);
	int Next(int min, int max);
	double NextDouble();
	double NextDouble(double max);
	double NextDouble(double min, double max);
private:
	const ulonglong m = 1442695040888963407;
	const ulonglong a = 6364136223846793005;
	ulonglong x = 0;
	int next();
};

