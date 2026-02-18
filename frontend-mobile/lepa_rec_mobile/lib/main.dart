import 'package:flutter/material.dart';
import 'package:lepa_rec_mobile/core/network/api_client.dart';

import 'core/widgets/home_page.dart';
import 'core/widgets/splash_router.dart';
import 'features/auth/presentation/pages/login_page.dart';

void main() {
  ApiClient.configure();
  runApp(const LepaRecApp());
}

class LepaRecApp extends StatelessWidget {
  const LepaRecApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Lepa reč',
      debugShowCheckedModeBanner: false,
      initialRoute: '/',
      routes: {
        '/': (_) => const SplashRouter(),
        '/login': (_) => const LoginPage(),
        '/home': (_) => const HomePage(),
      },
    );
  }
}
