#include "SFNetwork.h"
#include "ilRand.h"
#include "SFNetworkOscillator.h"

#include <vector>
#include <algorithm>
#include <random>
#include <fstream>


#define PI2				6.283185307179586476925286766559


namespace Diploma2 {


	SFNetworkOscillatorState::SFNetworkOscillatorState() {}
	SFNetworkOscillatorState::SFNetworkOscillatorState(double t, std::vector<double> p) {
		time = t;
		phases = p;
	}
	SFNetworkOscillatorState::~SFNetworkOscillatorState() {};
	void SFNetworkOscillatorState::Binarize(std::ofstream &str) {
		str.write((char*)&time, sizeof(time));

		int phase_count = phases.size();
		str.write((char*)&phase_count, sizeof(phase_count));
		for (double ph : phases)
			str.write((char*)&ph, sizeof(ph));
	}
	SFNetworkOscillatorState SFNetworkOscillatorState::Debinarize(std::ifstream & str)
	{
		double t;
		int p_cnt;
		str.read(reinterpret_cast<char *>(&t), sizeof(t));

		str.read(reinterpret_cast<char *>(&p_cnt), sizeof(p_cnt));
		std::vector<double> ph(p_cnt);
		for (int i = 0; i < p_cnt; i++)
			str.read(reinterpret_cast<char *>(&ph[i]), sizeof(ph[i]));

		return SFNetworkOscillatorState(t, ph);
	}



	void SFNetworkOscillator::constructor_funcs() {
		funcs = std::vector<RK4SFunc>(node_count);
		for (int i = 0; i < funcs.size(); i++) {
			funcs[i] = [this](int ind, double t, const std::vector<double>& x)->double {
				double res = 0;
				for (int j = 0; j < x.size(); j++)
				{
					//if (j == ind) continue;
					int w = 0;
					for (Edge edg : this->edges) {
						if (edg.from == j && edg.to == ind || edg.from == ind && edg.to == j)
						{
							w += edg.weight;
							break;
						}
					}
					res += w * sin(x[j] - x[ind]);
				}
				return phases[ind] + strength * res;
			};
		}
	}
	void SFNetworkOscillator::constructor(double str, double f_min, double f_max, double p_min, double p_max, double t_init, double t_step, double s_step) {
		strength = str;
		freq_init_min = f_min;
		freq_init_max = f_max;
		phase_init_min = p_min;
		phase_init_max = p_max;
		time_init = t_init;
		time = t_init;
		time_step = t_step;
		solve_step = s_step;

		freqs = std::vector<double>(node_count);
		phases = std::vector<double>(node_count);
		for (int i = 0; i < node_count; i++)
		{
			freqs[i] = ilRandom.Next((int)f_min, (int)f_max);
			phases[i] = ilRandom.NextDouble(p_min, p_max);
		}
		states = std::vector<SFNetworkOscillatorState>{ SFNetworkOscillatorState(time, phases) };
		constructor_funcs();
	}

	SFNetworkOscillator::SFNetworkOscillator() : SFNetwork() {}
	SFNetworkOscillator::SFNetworkOscillator(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step
	) {
		initialize(node_count, mlt, strength, freq_min, freq_max, phase_min, phase_max, time_init, time_step, solve_step);
	}
	SFNetworkOscillator::SFNetworkOscillator(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step, int random_seed
	) {
		initialize(node_count, mlt, strength, freq_min, freq_max, phase_min, phase_max, time_init, time_step, solve_step, random_seed);
	}
	SFNetworkOscillator::SFNetworkOscillator(int nodeCount, int mlt, int seed, ulong x, std::vector<Edge> &edges, double str, double fmin, double fmax, double pmin, double pmax, double ti, double ts, double ss, double t, std::vector<double> &ph, std::vector<double> &fr, std::vector<SFNetworkOscillatorState> &st
	) {
		initialize(nodeCount, mlt, seed, x, edges, str, fmin, fmax, pmin, pmax, ti, ts, ss, t, ph, fr, st);
	}
	SFNetworkOscillator::SFNetworkOscillator(const std::string &path) {
		initialize(path);
	}
	SFNetworkOscillator::~SFNetworkOscillator() {};



	void SFNetworkOscillator::initialize(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step) {
		initialize_sfnw(node_count, mlt);
		constructor(
			strength,
			freq_min,
			freq_max,
			phase_min,
			phase_max,
			time_init,
			time_step,
			solve_step
		);
	}
	void SFNetworkOscillator::initialize(int node_count, int mlt, double strength, double freq_min, double freq_max, double phase_min, double phase_max, double time_init, double time_step, double solve_step, int random_seed) {
		initialize_sfnw(node_count, mlt, random_seed);
		constructor(
			strength,
			freq_min,
			freq_max,
			phase_min,
			phase_max,
			time_init,
			time_step,
			solve_step
		);
	}
	void SFNetworkOscillator::initialize(int nodeCount, int mlt, int seed, ulong x, std::vector<Edge> &edges, double str, double fmin, double fmax, double pmin, double pmax, double ti, double ts, double ss, double t, std::vector<double> &ph, std::vector<double> &fr, std::vector<SFNetworkOscillatorState> &st) {
		initialize_sfnw(nodeCount, mlt, edges, ilRand(seed, x));

		strength = str;
		freq_init_min = fmin;
		freq_init_max = fmax;
		phase_init_min = pmin;
		phase_init_max = pmax;
		time_init = ti;
		time_step = ts;
		solve_step = ss;
		time = t;
		phases = ph;
		freqs = fr;
		states = st;

		constructor_funcs();
	}
	void SFNetworkOscillator::initialize(const std::string &path) {
		int n, m, s, e_cnt, p_cnt, f_cnt, s_cnt;
		ulong x;
		double stre,
			fmin,
			fmax,
			pmin,
			pmax,
			ti,
			ts,
			ss,
			t;

		std::ifstream str(path, std::ios::binary);
		char version = 0;
		str.read(reinterpret_cast<char *>(&version), sizeof(version));

		str.read(reinterpret_cast<char *>(&n), sizeof(n));
		str.read(reinterpret_cast<char *>(&m), sizeof(m));
		str.read(reinterpret_cast<char *>(&s), sizeof(s));
		str.read(reinterpret_cast<char *>(&x), sizeof(long long));

		str.read(reinterpret_cast<char *>(&e_cnt), sizeof(e_cnt));
		std::vector<Edge> edgs(e_cnt);
		for (int i = 0; i < e_cnt; i++)
			edgs[i] = Edge::Debinarize(str);

		str.read(reinterpret_cast<char *>(&stre), sizeof(stre));
		str.read(reinterpret_cast<char *>(&fmin), sizeof(fmin));
		str.read(reinterpret_cast<char *>(&fmax), sizeof(fmax));
		str.read(reinterpret_cast<char *>(&pmin), sizeof(pmin));
		str.read(reinterpret_cast<char *>(&pmax), sizeof(pmax));
		str.read(reinterpret_cast<char *>(&ti), sizeof(ti));
		str.read(reinterpret_cast<char *>(&ts), sizeof(ts));
		str.read(reinterpret_cast<char *>(&ss), sizeof(ss));
		str.read(reinterpret_cast<char *>(&t), sizeof(t));

		str.read(reinterpret_cast<char *>(&p_cnt), sizeof(p_cnt));
		std::vector<double> ph(p_cnt);
		for (int i = 0; i < p_cnt; i++)
			str.read(reinterpret_cast<char *>(&ph[i]), sizeof(ph[i]));

		str.read(reinterpret_cast<char *>(&f_cnt), sizeof(f_cnt));
		std::vector<double> fr(f_cnt);
		for (int i = 0; i < f_cnt; i++)
			str.read(reinterpret_cast<char *>(&fr[i]), sizeof(fr[i]));

		str.read(reinterpret_cast<char *>(&s_cnt), sizeof(s_cnt));
		std::vector<SFNetworkOscillatorState> st(s_cnt);
		for (int i = 0; i < s_cnt; i++)
			st[i] = SFNetworkOscillatorState::Debinarize(str);

		initialize(n, m, s, x, edgs, stre, fmin, fmax, pmin, pmax, ti, ts, ss, t, ph, fr, st);
	}


	void SFNetworkOscillator::phasesNormalize() {
		for (int i = 0; i < phases.size(); i++)
		{
			phases[i] = phases[i] - (int)(phases[i] / PI2) * PI2;
			if (phases[i] < 0) phases[i] = PI2 + phases[i];
		}
	}
	void SFNetworkOscillator::SimulateDynamicStep() {
		RK4SResult result = RK4S::Solve(funcs, time, phases, time + time_step, solve_step);
		time = result.time;
		phases = result.values;
		phasesNormalize();
		states.push_back(SFNetworkOscillatorState(time, phases));
	}


	void SFNetworkOscillator::Binarize(const char* path, const char version) {
		switch (version)
		{
		case 1:
			Binarize_v1(path); break;
		default: break;
		}
	}
	void SFNetworkOscillator::Binarize_v1(const char* path) {
		std::ofstream ofile(path, std::ios::binary);
		char version = 1;
		ofile.write(&version, sizeof(version));
		ofile.write((char*)&node_count, sizeof(node_count));
		ofile.write((char*)&multiplier, sizeof(multiplier));
		ofile.write((char*)&seed, sizeof(seed));
		ofile.write((char*)&ilRandom.x, sizeof(ilRandom.x));
		int edge_count = edges.size();
		ofile.write((char*)&edge_count, sizeof(edge_count));
		for (Edge edg : edges)
			edg.Binarize(ofile);

		ofile.write((char*)&strength, sizeof(strength));
		ofile.write((char*)&freq_init_min, sizeof(freq_init_min));
		ofile.write((char*)&freq_init_max, sizeof(freq_init_max));
		ofile.write((char*)&phase_init_min, sizeof(phase_init_min));
		ofile.write((char*)&phase_init_max, sizeof(phase_init_max));
		ofile.write((char*)&time_init, sizeof(time_init));
		ofile.write((char*)&time_step, sizeof(time_step));
		ofile.write((char*)&solve_step, sizeof(solve_step));
		ofile.write((char*)&time, sizeof(time));

		int phase_count = phases.size();
		ofile.write((char*)&phase_count, sizeof(phase_count));
		for (double ph : phases)
			ofile.write((char*)&ph, sizeof(ph));

		int freq_count = freqs.size();
		ofile.write((char*)&freq_count, sizeof(freq_count));
		for (double fr : freqs)
			ofile.write((char*)&fr, sizeof(fr));

		int state_count = states.size();
		ofile.write((char*)&state_count, sizeof(state_count));
		for (SFNetworkOscillatorState st : states)
			st.Binarize(ofile);
	}
}