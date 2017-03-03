#include "ilRand.h"

#include <ctime>
#include <limits>


#define uint unsigned int



ilRand::ilRand()
{
	seed = (int)(clock() % UINT_MAX);
	x = (ulonglong)(seed);
}
ilRand::ilRand(int s)
{
	x = seed = s;
}


ilRand::~ilRand()
{
}


ulonglong ilRand::next() {
	x = ((x * a) % m);
	return x;
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
	return (int)(next() % std::numeric_limits<int>::max()) % (max - min) + min;
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

void ilRand::Initialize() {
	x = seed = clock();
}
void ilRand::Initialize(int s) {
	x = seed = s;
}