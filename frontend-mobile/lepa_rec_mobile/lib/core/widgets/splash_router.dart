import 'package:flutter/material.dart';
import '../../features/auth/data/datasources/auth_session_reader.dart';

class SplashRouter extends StatefulWidget {
  const SplashRouter({super.key});

  @override
  State<SplashRouter> createState() => _SplashRouterState();
}

class _SplashRouterState extends State<SplashRouter> {
  @override
  void initState() {
    super.initState();
    _route();
  }

  Future<void> _route() async {
    final hasSession = await AuthSessionReader().hasSession();

    if (!mounted) return;

    Navigator.of(context).pushNamedAndRemoveUntil(
      hasSession ? '/home' : '/login',
      (_) => false,
    );
  }

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      body: Center(child: CircularProgressIndicator()),
    );
  }
}
