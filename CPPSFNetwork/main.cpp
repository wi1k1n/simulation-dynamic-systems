#include "SFNetwork.h"
#include "RK4S.h"
#include "ilRand.h"

#include <iostream>
#include <vector>
#include <algorithm>
#include <ctime>
#include <functional>
#include <random>
#include <fstream>


#define PI	3.1415926535897932384626433832795


using namespace std;




double measureRuntime(const function<void()> f) {
	clock_t start(clock());
	f();
	return (clock() - start) / (double)CLOCKS_PER_SEC;
}

void foo() {
	SFNetworkOscillator nw(300, 3, .65, 1, 10, -PI, PI, 0, 1, 0.01, 625);
	nw.Binarize("network.bin");
	return;
	vector<int> vec(3, 2);
	vec[0] = -3;
	vec[2] = 51;
	std::ofstream ofile("foo.bin", ios::binary);
	for (int i : vec)
		ofile.write((char*)&i, sizeof(i));

	return;
}

double start = clock();
int main(int argc, char* argv) {
	srand(start);
	double runtime = measureRuntime(foo);
	cout << endl << "Runtime: " << runtime << endl;
	system("pause");
	return 0;
}