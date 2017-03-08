#pragma once

#include "ilRand.h"
#include "RK4S.h"
#include "SFNetwork.h"

#include <vector>
#include <fstream>

#define ulong unsigned long long

namespace Diploma2 {


	class SFNetworkOscillatorState {
	public:
		double time;
		std::vector<double> phases;

		SFNetworkOscillatorState();
		SFNetworkOscillatorState(double, std::vector<double>);
		~SFNetworkOscillatorState();

		void Binarize(std::ofstream& str);
		static SFNetworkOscillatorState Debinarize(std::ifstream &str);
	};



	class SFNetworkOscillator : public SFNetwork {
	public:
		double strength,
			freq_init_min,
			freq_init_max,
			phase_init_min,
			phase_init_max,
			time_init,
			time_step,
			solve_step,

			time;

		std::vector<double> phases,
			freqs;
		std::vector<SFNetworkOscillatorState> states;

		SFNetworkOscillator();
		SFNetworkOscillator(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step);
		SFNetworkOscillator(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step, int random_seed);
		SFNetworkOscillator(int nodeCount, int mlt, int seed, ulong x, std::vector<Edge> &edges, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step, double time, std::vector<double> &phases, std::vector<double> &freqs, std::vector<SFNetworkOscillatorState> &states);
		SFNetworkOscillator(const std::string &path);
		~SFNetworkOscillator();

		void initialize(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step);
		void initialize(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step, int random_seed);
		void initialize(int nodeCount, int mlt, int seed, ulong x, std::vector<Edge> &edges, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step, double time, std::vector<double> &phases, std::vector<double> &freqs, std::vector<SFNetworkOscillatorState> &states);
		void initialize(const std::string &path);

		void SimulateDynamicStep();

		void Binarize(const char* path, const char version);
	private:
		std::vector<RK4SFunc> funcs;

		void constructor_funcs();
		void constructor(double str, double f_min, double f_max, double p_min, double p_max, double t_init, double t_step, double s_step);
		void phasesNormalize();

		void Binarize_v1(const char* path);
	};

}