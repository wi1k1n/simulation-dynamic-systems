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
	double start_local = clock();
	SFNetworkOscillator nw(75, 3, .65, 1, 10, -PI, PI, 0, 1, 0.01, 625);
	cout << "Network generated in " << clock() - start_local << "ms" << endl;
	for (int i = 0; i < 2; i++) {
		start_local = clock();
		nw.SimulateDynamicStep();
		cout << "Dynamic:\ttime: " << nw.time << "\t" << clock() - start_local << "ms" << endl;
	}
	nw.Binarize("network_50_3.bin", 1);
	cout << "Network successfully binarized. Press any key to resume." << endl;
	int t;
	cin >> t;
	for (int i = 0; i < 5; i++) {
		start_local = clock();
		nw.SimulateDynamicStep();
		cout << "Dynamic:\ttime: " << nw.time << "\t" << clock() - start_local << "ms" << endl;
	}
	cout << endl << "time: " << nw.time << endl;
	for (double d : nw.phases)
		cout << d << endl;
}

double start = clock();
int main(int argc, char* argv) {
	srand(start);
	double runtime = measureRuntime(foo);
	cout << endl << "Runtime: " << runtime << endl;
	system("pause");
	return 0;
}