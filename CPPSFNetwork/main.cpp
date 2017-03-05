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
#include <thread>
#include <Windows.h>
#include <string>


#define PI	3.1415926535897932384626433832795


using namespace std;




double measureRuntime(const function<void()> f) {
	clock_t start(clock());
	f();
	return (clock() - start) / (double)CLOCKS_PER_SEC;
}

bool stop = false;
void cli() {
	while (!stop) {
		int t;
		cin >> t;
		stop = t == 9;
	}
}
void foo() {
	double start_local = clock();
	SFNetworkOscillator nw(900, 3, .2, 1, 10, -PI, PI, 0, .1, .005, 625);
	cout << "Network generated in " << clock() - start_local << "ms" << endl;
	while(!stop) {
		start_local = clock();
		nw.SimulateDynamicStep();
		cout << "Dynamic:\ttime: " << nw.time << "\t" << clock() - start_local << "ms" << endl;
	}
	nw.Binarize(("network_900_3_.2_1_10_-pi_pi_0_.1_.005_625" + to_string(nw.states.size()) + ".bin").data(), 1);
	cout << "Network successfully binarized. Press any key to resume." << endl;
}

double start = clock();
int main(int argc, char* argv) {
	srand(start);
	double runtime = measureRuntime([]()->void {
		thread calculation(&foo);
		thread cli(&cli);
		cli.join();
		calculation.join();
	});
	cout << endl << "Runtime: " << runtime << endl;
	system("pause");
	return 0;
}