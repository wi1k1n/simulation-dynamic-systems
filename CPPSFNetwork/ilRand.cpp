#include "ilRand.h"

#include <ctime>
#include <limits>


#define uint unsigned int



ilRand::ilRand()
{
	x = (ulonglong)(clock() % UINT_MAX);
}
ilRand::ilRand(int seed)
{
	x = seed;
}


ilRand::~ilRand()
{
}


int ilRand::next() {
	x = ((x * a) % m);
	return (int)(x % std::numeric_limits<int>::max());
}

int ilRand::Next()
{
	return Next(0, std::numeric_limits<int>::max());
}
int ilRand::Next(int max)
{
	return Next(0, max);
}
int ilRand::Next(int min, int max)
{
	return next() % (max - min) + min;
}

double ilRand::NextDouble()
{
	return NextDouble(0, std::numeric_limits<double>::max());
}
double ilRand::NextDouble(double max)
{
	return NextDouble(0, max);
}
double ilRand::NextDouble(double min, double max)
{
	return (next() / (double)m) * (max - min) + min;
}