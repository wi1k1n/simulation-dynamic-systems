#include "SFNetwork.h"
#include "RK4S.h"
#include "ilRand.h"

#include <iostream>
#include <vector>
#include <algorithm>
#include <ctime>
#include <functional>
#include <random>


#define PI				3.1415926535897932384626433832795


using namespace std;




double measureRuntime(const function<void()> f) {
	clock_t start(clock());
	f();
	return (clock() - start) / (double)CLOCKS_PER_SEC;
}

int main(int argc, char* argv) {
	freopen("ouput.txt", "w", stdout);
	double start = clock();
	srand(start);
	double runtime = measureRuntime([start]()->void {
		ilRand rnd(170303);
		SFNetwork nww(100, 3);
		for (int i = 0; i < nww.edges.size(); i++)
			cout << nww.edges[i].from << "\t" << nww.edges[i].to << "\t" << nww.edges[i].weight << "\n";
		return;
		SFNetworkOscillator nw(100, 3, .65, 1, 10, -PI, PI, 0, .1, .001, 170302);
		for (int i = 0; i < 100; i++) {
			double local_start = clock();
			nw.SimulateDynamicStep();
			cout << "Time: " << nw.time << "\t" << (clock() - local_start)/1000.0 << "\t" << (clock() - start)/1000.0 << endl;
		}
	});
	cout << endl << "Runtime: " << runtime;
	system("pause");
	return 0;
}